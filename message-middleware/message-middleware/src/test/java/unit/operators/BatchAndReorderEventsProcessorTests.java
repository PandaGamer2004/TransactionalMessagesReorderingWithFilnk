package unit.operators;

import org.apache.flink.api.common.typeinfo.Types;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.JsonNode;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.ObjectMapper;
import org.apache.flink.streaming.api.operators.ProcessOperator;
import org.apache.flink.streaming.util.KeyedOneInputStreamOperatorTestHarness;
import org.apache.flink.streaming.util.OneInputStreamOperatorTestHarness;
import org.daniil.models.BatchedRocketUpdateModel;
import org.daniil.models.RocketUpdateFlatModel;
import org.daniil.models.RocketUpdateFlatModelSupplier;
import org.daniil.processors.BatchAndReorderEventsProcessor;
import org.daniil.projectors.FromFlatModelMapper;
import org.daniil.selectors.ChannelKeySelector;
import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;

import java.time.OffsetDateTime;
import java.util.Arrays;
import java.util.stream.Collectors;
import java.util.stream.Stream;

public class BatchAndReorderEventsProcessorTests {
    private JsonNode messageNode;
    private OneInputStreamOperatorTestHarness<RocketUpdateFlatModel, BatchedRocketUpdateModel> rocketUpdateStream;
    private RocketUpdateFlatModelSupplier updateModelSupplier;

    @Before
    public void prepareTestHarnesses() throws Exception {
        this.messageNode = new ObjectMapper()
                .readTree( "{\"by\": \"3000\"}");

        this.updateModelSupplier = num -> new RocketUpdateFlatModel(
                messageNode, "channel-1", num, OffsetDateTime.now(), "sample-type"
        );

        this.rocketUpdateStream = new KeyedOneInputStreamOperatorTestHarness<>(
                new ProcessOperator<>(new BatchAndReorderEventsProcessor(new FromFlatModelMapper())),
                new ChannelKeySelector(),
                Types.STRING
        );

        this.rocketUpdateStream.open();
    }



    public Iterable<Stream<RocketUpdateFlatModel>> fromTuples(Tuple2<Integer, Integer>... definition){
        return Arrays
                .stream(definition).map(it -> RocketUpdateFlatModel.initStream(it.f0, it.f1, updateModelSupplier))
                .collect(Collectors.toList());
    }
    @Test
    public void duringOutOfOrderExecutionShouldEmitBatchesInCorrectOrder() throws Exception{

        var projectedStreams = fromTuples(
                Tuple2.of(10, 15),
                Tuple2.of(15, 20),
                Tuple2.of(1, 10),
                Tuple2.of(25, 30),
                Tuple2.of(22, 25),
                Tuple2.of(20, 22)
        );

        for(var stream: projectedStreams){
            ApplyStreamToHarness(stream, 10);
        }

        rocketUpdateStream.setProcessingTime(10);

        var projectedModels = rocketUpdateStream
                .extractOutputValues()
                .stream()
                .flatMap(it -> Arrays.stream(it.getBatchedModels()))
                .collect(Collectors.toList());

        for(int i = 1; i < projectedModels.size(); i++){
            var current = projectedModels.get(i);
            var previous = projectedModels.get(i - 1);

            Assert.assertEquals(current.getMetadata().getMessageNumber(), previous.getMetadata().getMessageNumber() + 1);
        }
    }
    @Test
    public void shouldEmitOnlyOneBatchDuringOutOfOrderExecution() throws Exception {

        //Basically getting each third message of the stream
        var projectedStream = RocketUpdateFlatModel.initStream(
                        1, 11, updateModelSupplier
                )
                .filter(it -> it.getMessageNumber() == 1 || it.getMessageNumber() % 3 == 0);


        ApplyStreamToHarness(projectedStream, 10);
        rocketUpdateStream.setProcessingTime(10);
        Assert.assertEquals( 1, rocketUpdateStream.getOutput().size());
    }

    private void ApplyStreamToHarness(Stream<RocketUpdateFlatModel> flatModelStream, int executionTimestamp) throws Exception {

        var collectedStream = flatModelStream.collect(Collectors.toList());
        for (var flatUpdateModel : collectedStream) {
            rocketUpdateStream.processElement(flatUpdateModel, executionTimestamp);
        }
    }
}
