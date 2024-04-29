package org.daniil.processors;

import org.apache.flink.api.common.functions.OpenContext;
import org.apache.flink.api.common.state.ValueState;
import org.apache.flink.api.common.state.ValueStateDescriptor;
import org.apache.flink.api.common.typeinfo.Types;
import org.apache.flink.streaming.api.functions.ProcessFunction;
import org.apache.flink.util.Collector;
import org.daniil.models.BatchedRocketUpdateModel;
import org.daniil.models.RocketUpdateFlatModel;
import org.daniil.models.RocketUpdateModel;
import org.daniil.projectors.FromFlatModelMapper;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;

public class BatchAndReorderEventsProcessor extends ProcessFunction<RocketUpdateFlatModel, BatchedRocketUpdateModel> {

    private static final Integer InitialOrder = 1;
    private transient ValueState<List<RocketUpdateFlatModel>> rocketUpdateFlatModelState;

    private transient ValueState<Integer> lastSendOrderState;

    public BatchAndReorderEventsProcessor(){
    }
    @Override
    public void open(OpenContext openContext) throws Exception {
        ValueStateDescriptor<List<RocketUpdateFlatModel>> updateModelsBufferDescriptor
                = new ValueStateDescriptor<List<RocketUpdateFlatModel>>(
                "rocket-updates-flat-model",
                Types.LIST(Types.POJO(RocketUpdateFlatModel.class))
        );

        ValueStateDescriptor<Integer> lastSendOrderDescriptor = new ValueStateDescriptor<>(
                "rocket-updates-last",
                Types.INT
        );

        rocketUpdateFlatModelState = getRuntimeContext()
                .getState(updateModelsBufferDescriptor);

        lastSendOrderState = getRuntimeContext()
                .getState(lastSendOrderDescriptor);
    }

    @Override
    public void processElement(RocketUpdateFlatModel rocketUpdateFlatModel, ProcessFunction<RocketUpdateFlatModel, BatchedRocketUpdateModel>.Context context, Collector<BatchedRocketUpdateModel> collector) throws Exception {

        var rawLastSendValue
                = lastSendOrderState.value();

        var resultLastSendValue
                = rawLastSendValue == null? InitialOrder: rawLastSendValue;

        List<RocketUpdateFlatModel> mergedStateValues = mergeIncomingWithStateValue(rocketUpdateFlatModel);

        var lowestByOrderEvent = mergedStateValues.get(0);

        //Means that we should skip that batch
        if(lowestByOrderEvent.getMessageNumber() > resultLastSendValue + 1){
            this.rocketUpdateFlatModelState.update(mergedStateValues);
            return;
        }

        ContiniousBatchWithPosition result
                = extractContinuousBatchWithPosition(lowestByOrderEvent, mergedStateValues);

        var biggestOrderInBatch
                = result.incomingBatch
                .get(result.incomingBatch.size() -1)
                .getMessageNumber();

        this.lastSendOrderState.update(biggestOrderInBatch);

        BatchedRocketUpdateModel resultingBatch = getBatchedRocketUpdateModel(
                rocketUpdateFlatModel.getChannel(),
                result
        );

        var dataToSuspend
                = mergedStateValues
                .stream()
                .skip(result.leftCornerPoint)
                .collect(Collectors.toList());

        rocketUpdateFlatModelState.update(dataToSuspend);

        collector.collect(resultingBatch);
    }

    private BatchedRocketUpdateModel getBatchedRocketUpdateModel(String channelId, ContiniousBatchWithPosition result) {
        var mapper = new FromFlatModelMapper();
        var projectedBatch = result.incomingBatch
                .stream()
                .map(flatModel -> mapper.map(flatModel))
                .toArray(RocketUpdateModel[]::new);


        BatchedRocketUpdateModel resultingBatch = new BatchedRocketUpdateModel(
                channelId,
                projectedBatch
        );
        return resultingBatch;
    }

    private static ContiniousBatchWithPosition extractContinuousBatchWithPosition(
            RocketUpdateFlatModel lowestByOrderEvent,
            List<RocketUpdateFlatModel> mergedStateValues
    ) {
        List<RocketUpdateFlatModel> incomingBatch = new ArrayList<>();
        incomingBatch.add(lowestByOrderEvent);

        int i;
        for (i = 1; i < mergedStateValues.size(); i++) {
            var current = mergedStateValues.get(i);
            var previous = mergedStateValues.get(i - 1);
            if (previous.getMessageNumber() == current.getMessageNumber() - 1) {
                incomingBatch.add(current);
            } else {
                break;
            }
        }
        ContiniousBatchWithPosition result = new ContiniousBatchWithPosition(incomingBatch, i);
        return result;
    }

    private static class ContiniousBatchWithPosition {
        public final List<RocketUpdateFlatModel> incomingBatch;
        public final int leftCornerPoint;

        public ContiniousBatchWithPosition(List<RocketUpdateFlatModel> incomingBatch, int i) {
            this.incomingBatch = incomingBatch;
            this.leftCornerPoint = i;
        }
    }

    private List<RocketUpdateFlatModel> mergeIncomingWithStateValue(RocketUpdateFlatModel rocketUpdateFlatModel) throws IOException {
        var rawStateValue
                = rocketUpdateFlatModelState.value();

        List<RocketUpdateFlatModel> resultStateValue =
                rawStateValue == null ? new ArrayList<>() : rawStateValue;

        resultStateValue.add(rocketUpdateFlatModel);
        resultStateValue.sort(Comparator.comparingInt(RocketUpdateFlatModel::getMessageNumber));
        return resultStateValue;
    }
}