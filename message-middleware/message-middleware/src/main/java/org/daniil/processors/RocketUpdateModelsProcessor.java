package org.daniil.processors;

import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.checkerframework.checker.units.qual.C;
import org.daniil.models.*;
import org.daniil.projectors.FromFlatModelMapper;
import org.daniil.projectors.ToFlatModelMapper;
import org.daniil.selectors.ChannelKeySelector;

public class RocketUpdateModelsProcessor {
    public DataStream<BatchedRocketUpdateModel> registerProcessing(DataStream<RocketUpdateModel> modelsStream) {
        DataStream<BatchedRocketUpdateModel> resultStream = modelsStream
                .map(
                        (MapFunction<RocketUpdateModel, RocketUpdateFlatModel>)
                                rocketUpdateModel -> new ToFlatModelMapper().map(rocketUpdateModel)
                )
                .keyBy(new ChannelKeySelector())
                .filter(new FilterDuplicatedEventsFunction())
                .keyBy(new ChannelKeySelector())
                .process(new BatchAndReorderEventsProcessor())
                .name("reordered-and-deduplicator");
        return resultStream;
    }



}
