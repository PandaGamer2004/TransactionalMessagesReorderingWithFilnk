package org.daniil.models;

import org.apache.flink.formats.json.JsonSerializationSchema;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.DeserializationFeature;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.MapperFeature;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.PropertyNamingStrategy;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.SerializationFeature;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;
import org.apache.flink.util.jackson.JacksonMapperFactory;

import java.lang.reflect.Array;
import java.util.List;

public class BatchedRocketUpdateModel {
    private String channelId;

    private RocketUpdateModel[] batchedModels;

    public BatchedRocketUpdateModel(
            String channelId,
            RocketUpdateModel[] batchedModels
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

    public RocketUpdateModel[] getBatchedModels() {
        return batchedModels;
    }

    public void setBatchedModels(RocketUpdateModel[] batchedModels) {
        this.batchedModels = batchedModels;
    }

    public static JsonSerializationSchema<BatchedRocketUpdateModel> getSerializationSchema(){
       return new JsonSerializationSchema<>(() -> {
           var mapper = JacksonMapperFactory.createObjectMapper();
           mapper.setPropertyNamingStrategy(PropertyNamingStrategy.LOWER_CAMEL_CASE);
           mapper.disable(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS);
           mapper.registerModule(new JavaTimeModule());
           return mapper;
       });
    }

}
