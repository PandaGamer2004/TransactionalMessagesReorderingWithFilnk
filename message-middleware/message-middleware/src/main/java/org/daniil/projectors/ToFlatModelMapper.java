package org.daniil.projectors;

import org.daniil.models.Mapper;
import org.daniil.models.RocketUpdateFlatModel;
import org.daniil.models.RocketUpdateModel;

public class ToFlatModelMapper implements Mapper<RocketUpdateModel, RocketUpdateFlatModel> {
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