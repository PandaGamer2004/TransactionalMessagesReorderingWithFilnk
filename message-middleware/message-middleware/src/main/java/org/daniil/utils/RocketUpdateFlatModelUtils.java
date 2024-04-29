package org.daniil.utils;

import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.JsonNode;
import org.daniil.models.RocketUpdateFlatModel;
import org.daniil.models.RocketUpdateFlatModelSupplier;
import org.daniil.models.RocketUpdateModel;

import java.time.OffsetDateTime;
import java.util.stream.IntStream;
import java.util.stream.Stream;

public class RocketUpdateFlatModelUtils {

    public static RocketUpdateFlatModel createRocketUpdateFlatModel(
            JsonNode message,
            String channel,
            int messageNumber,
            OffsetDateTime messageTime,
            String messageType
    ) {
        var model = new RocketUpdateFlatModel();
        model.setMessage(message);
        model.setChannel(channel);
        model.setMessageNumber(messageNumber);
        model.setMessageTime(messageTime);
        model.setMessageType(messageType);
        return model;
    }

    public static Stream<RocketUpdateFlatModel> initStream(
            Integer from,
            Integer to,
            RocketUpdateFlatModelSupplier modelSupplier
    ){
        return IntStream.range(from, to).mapToObj(modelSupplier::create);
    }

}
