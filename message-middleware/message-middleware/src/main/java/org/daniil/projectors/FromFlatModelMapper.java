package org.daniil.projectors;

import org.daniil.models.Mapper;
import org.daniil.models.RocketUpdateFlatModel;
import org.daniil.models.RocketUpdateModel;
import org.daniil.models.RocketUpdateModelMetadata;

public class FromFlatModelMapper implements Mapper<RocketUpdateFlatModel, RocketUpdateModel> {

    public RocketUpdateModel map(RocketUpdateFlatModel flatModel) {
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
    }
}
