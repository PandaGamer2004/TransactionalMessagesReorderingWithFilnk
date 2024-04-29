package org.daniil.projectors;

import org.apache.flink.api.common.functions.MapFunction;
import org.daniil.models.RocketUpdateFlatModel;
import org.daniil.models.RocketUpdateModel;

public class ToFlatModel implements MapFunction<RocketUpdateModel, RocketUpdateFlatModel> {

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