package org.daniil.models;

import java.time.OffsetDateTime;

public class RocketUpdateModelMetadata {
    private String channel;

    private int messageNumber;

    private OffsetDateTime messageTime;

    private String messageType;

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
