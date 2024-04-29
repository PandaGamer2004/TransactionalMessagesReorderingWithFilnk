package org.daniil.selectors;

import org.apache.flink.api.java.functions.KeySelector;
import org.daniil.models.RocketUpdateFlatModel;

public class ChannelKeySelector implements KeySelector<RocketUpdateFlatModel, String> {
    @Override
    public String getKey(RocketUpdateFlatModel flatModel) {
        return flatModel.getChannel();
    }
}
