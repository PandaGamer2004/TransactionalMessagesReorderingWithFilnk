package org.daniil.processors;

import org.apache.commons.lang3.NotImplementedException;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.daniil.models.BatchedRocketUpdateModel;
import org.daniil.models.RocketUpdateModel;

public class RocketUpdateModelsProcessor {
    public DataStream<BatchedRocketUpdateModel> registerProcessing(DataStream<RocketUpdateModel> modelsStream){
        throw new NotImplementedException();
    }
}
