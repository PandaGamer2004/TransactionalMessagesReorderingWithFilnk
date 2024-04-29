package org.daniil.processors;

import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.daniil.models.*;
import org.daniil.projectors.FromFlatModelMapper;
import org.daniil.projectors.ToFlatModelMapper;

public class RocketUpdateModelsProcessor {
    public DataStream<BatchedRocketUpdateModel> registerProcessing(DataStream<RocketUpdateModel> modelsStream) {
        DataStream<BatchedRocketUpdateModel> resultStream = modelsStream
                .map(
                        (MapFunction<RocketUpdateModel, RocketUpdateFlatModel>)
                                rocketUpdateModel -> new ToFlatModelMapper().map(rocketUpdateModel)
                )
                .keyBy(RocketUpdateFlatModel::getChannel)
                .filter(new FilterDuplicatedEventsFunction())
                .process(new BatchAndReorderEventsProcessor(new FromFlatModelMapper()))
                .name("reordered-and-deduplicator");
        return resultStream;
    }



}
