package unit.operators;

import org.apache.flink.api.common.typeinfo.Types;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.JsonNode;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.ObjectMapper;
import org.apache.flink.streaming.api.operators.StreamFilter;
import org.apache.flink.streaming.util.KeyedOneInputStreamOperatorTestHarness;
import org.apache.flink.streaming.util.OneInputStreamOperatorTestHarness;
import org.daniil.models.RocketUpdateFlatModel;
import org.daniil.processors.FilterDuplicatedEventsFunction;
import org.daniil.selectors.ChannelKeySelector;
import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;

import java.time.OffsetDateTime;


public class FilterDuplicateEventsProcessorTests {
    OneInputStreamOperatorTestHarness<RocketUpdateFlatModel, RocketUpdateFlatModel> testHarness;
    FilterDuplicatedEventsFunction filterDuplicatesFunction;


    @Before
    public void Setup() throws Exception {
         filterDuplicatesFunction = new FilterDuplicatedEventsFunction();

         testHarness = new KeyedOneInputStreamOperatorTestHarness<>(
                 new StreamFilter<>(filterDuplicatesFunction),
                 new ChannelKeySelector(),
                 Types.STRING
         );

         testHarness.open();
    }


    @Test
    public void FilterFunctionOnUniqueInputShouldProduceAll() throws Exception {
        JsonNode messageNode = new ObjectMapper()
                .readTree( "{\"by\": \"3000\"}");

        String channel = "channel1";
        String type = "change";
        var event1 = new RocketUpdateFlatModel(
                messageNode,
                channel,
                1,
                OffsetDateTime.now(),
                type);
        var event2 = new RocketUpdateFlatModel(
                messageNode,
                channel,
                2,
                OffsetDateTime.now(),
                type);
        testHarness.processElement(event1, 10);
        testHarness.processElement(event2, 10);
        testHarness.setProcessingTime(10);
        Assert.assertEquals(testHarness.getOutput().size(), 2);

    }

    @Test
    public void FilterFunctionOnSameEventsShouldDeduplicate() throws Exception {
        JsonNode messageNode = new ObjectMapper()
                .readTree( "{\"by\": \"3000\"}");

        String channel = "channel1";
        String type = "change";
        var event1 = new RocketUpdateFlatModel(
                messageNode,
                channel,
                1,
                OffsetDateTime.now(),
                type);

        testHarness.processElement(event1, 10);
        testHarness.processElement(event1, 10);
        testHarness.setProcessingTime(10);
        Assert.assertEquals(testHarness.getOutput().size(), 1);

    }


}
