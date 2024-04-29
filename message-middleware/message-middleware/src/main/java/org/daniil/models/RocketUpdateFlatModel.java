package org.daniil.models;


import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.JsonNode;

import java.time.OffsetDateTime;
import java.util.stream.IntStream;
import java.util.stream.Stream;

public class RocketUpdateFlatModel {
    JsonNode message;

    private String channel;

    private int messageNumber;

    private OffsetDateTime messageTime;

    private String messageType;

    public RocketUpdateFlatModel() {
    }

    public RocketUpdateFlatModel(JsonNode message, String channel, int messageNumber, OffsetDateTime messageTime, String messageType) {
        this.message = message;
        this.channel = channel;
        this.messageNumber = messageNumber;
        this.messageTime = messageTime;
        this.messageType = messageType;
    }

    public static Stream<RocketUpdateFlatModel> initStream(
            Integer from,
            Integer to,
            RocketUpdateFlatModelSupplier modelSupplier
    ){
        return IntStream.range(from, to).mapToObj(modelSupplier::create);
    }

    public JsonNode getMessage() {
        return message;
    }

    public void setMessage(JsonNode message) {
        this.message = message;
    }

    public String getChannel() {
        return channel;
    }

    public void setChannel(String channel) {
        this.channel = channel;
    }

    public int getMessageNumber() {
        return messageNumber;
    }

    public void setMessageNumber(int messageNumber) {
        this.messageNumber = messageNumber;
    }

    public OffsetDateTime getMessageTime() {
        return messageTime;
    }

    public void setMessageTime(OffsetDateTime messageTime) {
        this.messageTime = messageTime;
    }

    public String getMessageType() {
        return messageType;
    }

    public void setMessageType(String messageType) {
        this.messageType = messageType;
    }


}
