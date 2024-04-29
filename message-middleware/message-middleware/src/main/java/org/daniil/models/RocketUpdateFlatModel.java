package org.daniil.models;


import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.JsonNode;

import java.time.OffsetDateTime;

public class RocketUpdateFlatModel {
    JsonNode message;

    private String channel;

    private int messageNumber;

    private OffsetDateTime messageTime;

    private String messageType;



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
