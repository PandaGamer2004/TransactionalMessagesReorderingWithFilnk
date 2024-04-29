package org.daniil.models;

import org.apache.flink.formats.json.JsonDeserializationSchema;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.*;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;
import org.apache.flink.util.function.SerializableSupplier;
import org.apache.flink.util.jackson.JacksonMapperFactory;

public class RocketUpdateModel {


    private RocketUpdateModelMetadata metadata;

    //For out case message content is not really important
    //As identification that is sort of generic json node that should
    //Be treated not as string, in future maybe i will override it
    private JsonNode message;

    public JsonNode getMessage() {
        return message;
    }

    public void setMessage(JsonNode message) {
        this.message = message;
    }

    public RocketUpdateModelMetadata getMetadata() {
        return metadata;
    }

    public void setMetadata(RocketUpdateModelMetadata metadata) {
        this.metadata = metadata;
    }

    public static JsonDeserializationSchema<RocketUpdateModel> getSchema(){
        return new JsonDeserializationSchema<>(
                RocketUpdateModel.class,
                getObjectMapperForUpdateModel()
        );
    }

    private static SerializableSupplier<ObjectMapper> getObjectMapperForUpdateModel()
    {
        return () -> {
            var baseMapper = JacksonMapperFactory.createObjectMapper();
            baseMapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
            baseMapper.configure(MapperFeature.ACCEPT_CASE_INSENSITIVE_PROPERTIES, true);
            baseMapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, true);
            baseMapper.disable(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS);
            baseMapper.registerModule(new JavaTimeModule());
            return baseMapper;
        };
    }
}
