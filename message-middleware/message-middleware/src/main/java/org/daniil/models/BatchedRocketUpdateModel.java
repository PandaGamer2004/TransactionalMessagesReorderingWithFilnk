package org.daniil.models;

import org.apache.flink.formats.json.JsonSerializationSchema;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.PropertyNamingStrategy;
import org.apache.flink.util.jackson.JacksonMapperFactory;

import java.util.List;

public class BatchedRocketUpdateModel {
    private String channelId;

    private List<RocketUpdateModel> batchedModels;

    public BatchedRocketUpdateModel(
            String channelId,
            List<RocketUpdateModel> batchedModels
    ){
        this.channelId = channelId;
        this.batchedModels = batchedModels;
    }
    public String getChannelId() {
        return channelId;
    }

    public void setChannelId(String channelId) {
        this.channelId = channelId;
    }

    public List<RocketUpdateModel> getBatchedModels() {
        return batchedModels;
    }

    public void setBatchedModels(List<RocketUpdateModel> batchedModels) {
        this.batchedModels = batchedModels;
    }

    public static JsonSerializationSchema<BatchedRocketUpdateModel> getSerializationSchema(){
       return  new JsonSerializationSchema<BatchedRocketUpdateModel>(() -> {
            var mapper = JacksonMapperFactory.createObjectMapper();
            mapper.setPropertyNamingStrategy(PropertyNamingStrategy.LOWER_CAMEL_CASE);
            return mapper;
        });
    }

}
