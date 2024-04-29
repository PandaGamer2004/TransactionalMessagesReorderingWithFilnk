package org.daniil.processors;

import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.common.functions.OpenContext;
import org.apache.flink.api.common.state.ValueState;
import org.apache.flink.api.common.state.ValueStateDescriptor;
import org.apache.flink.api.common.typeinfo.Types;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.functions.KeyedProcessFunction;
import org.apache.flink.util.Collector;
import org.daniil.models.BatchedRocketUpdateModel;
import org.daniil.models.RocketUpdateFlatModel;
import org.daniil.models.RocketUpdateModel;
import org.daniil.models.RocketUpdateModelMetadata;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;

public class RocketUpdateModelsProcessor {
    public DataStream<BatchedRocketUpdateModel> registerProcessing(DataStream<RocketUpdateModel> modelsStream){
        modelsStream
            .map(new ToFlatModel())
            .keyBy(RocketUpdateFlatModel::getChannel)
            .process(new BatchAndReorderEventsProcessor());
    }


    private static class BatchAndReorderEventsProcessor extends KeyedProcessFunction<String, RocketUpdateFlatModel, BatchedRocketUpdateModel>{

        private transient ValueState<List<RocketUpdateFlatModel>> rocketUpdateFlatModelState;

        @Override
        public void processElement(
                RocketUpdateFlatModel rocketUpdateFlatModel,
                KeyedProcessFunction<String, RocketUpdateFlatModel, BatchedRocketUpdateModel>.Context context,
                Collector<BatchedRocketUpdateModel> collector
        ) throws Exception {
            var rawStateValue = rocketUpdateFlatModelState.value();
            List<RocketUpdateFlatModel> resultStateValue =
                    rawStateValue == null? new ArrayList<>(): rawStateValue;
            resultStateValue.add(rocketUpdateFlatModel);
            Collections.sort(
                    resultStateValue,
                    Comparator.comparingInt(RocketUpdateFlatModel::getMessageNumber)
            );

            List<RocketUpdateFlatModel> incomingBatch = List.of(resultStateValue.get(0));

            int i;
            for(i = 1; i < resultStateValue.size(); i++){
                var current = resultStateValue.get(i);
                var previous = resultStateValue.get(i - 1);
                if(previous.getMessageNumber() == current.getMessageNumber() - 1){
                    incomingBatch.add(current);
                }else{
                    break;
                }
            }

            var projectedBatch = incomingBatch.stream().map(flatModel -> {
                var rocketModel = new RocketUpdateModel();
                rocketModel.setMessage(
                        flatModel.getMessage()
                );

                var metadata = new RocketUpdateModelMetadata();
                metadata.setChannel(flatModel.getChannel());
                metadata.setMessageNumber(flatModel.getMessageNumber());
                metadata.setMessageTime(flatModel.getMessageTime());
                metadata.setMessageType(flatModel.getMessageType());

                rocketModel.setMetadata(metadata);
                return rocketModel;
            }).collect(Collectors.toList());

            BatchedRocketUpdateModel resultingBatch = new BatchedRocketUpdateModel(
                    context.getCurrentKey(),
                    projectedBatch
            );

            var dataToSuspend
                    = resultStateValue.stream().skip(i).collect(Collectors.toList());

            rocketUpdateFlatModelState.update(dataToSuspend);

            collector.collect(resultingBatch);
        }

        @Override
        public void open(OpenContext openContext) throws Exception {
            ValueStateDescriptor<List<RocketUpdateFlatModel>> descriptor
                    = new ValueStateDescriptor<List<RocketUpdateFlatModel>>(
                    "rocket-updates-flat-model",
                    Types.LIST(Types.POJO(RocketUpdateFlatModel.class))
            );

            rocketUpdateFlatModelState =  getRuntimeContext().getState(descriptor);
        }
    }
    private static class ToFlatModel implements MapFunction<RocketUpdateModel, RocketUpdateFlatModel> {

        @Override
        public RocketUpdateFlatModel map(RocketUpdateModel rocketUpdateModel) {
            var resultModel = new RocketUpdateFlatModel();
            var metadata = rocketUpdateModel.getMetadata();

            resultModel.setMessage(rocketUpdateModel.getMessage());
            resultModel.setChannel(metadata.getChannel());
            resultModel.setMessageNumber(metadata.getMessageNumber());
            resultModel.setMessageTime(metadata.getMessageTime());
            resultModel.setMessageType(metadata.getMessageType());
            return resultModel;
        }
    }
}
