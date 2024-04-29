package org.daniil.models;

import org.apache.commons.lang3.NotImplementedException;
import org.apache.flink.formats.json.JsonSerializationSchema;

import java.util.List;

public class BatchedRocketUpdateModel {
    private String channelId;

    private List<RocketUpdateModel> ReorderedBatch;
    public String getChannelId() {
        return channelId;
    }

    public void setChannelId(String channelId) {
        this.channelId = channelId;
    }


    public static JsonSerializationSchema<BatchedRocketUpdateModel> getSerializationSchema(){
        throw new NotImplementedException();
    }

}
