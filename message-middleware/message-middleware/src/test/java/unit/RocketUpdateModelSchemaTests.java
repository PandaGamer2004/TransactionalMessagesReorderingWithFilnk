package unit;

import org.apache.flink.api.common.serialization.DeserializationSchema;
import org.apache.flink.metrics.MetricGroup;
import org.apache.flink.util.UserCodeClassLoader;
import org.daniil.models.RocketUpdateModel;
import org.junit.Assert;
import org.junit.Test;
import unit.mocks.InitializationContextMock;

import java.io.IOException;
import java.nio.charset.StandardCharsets;

public class RocketUpdateModelSchemaTests {

    private String samplePaylod = "{\"Metadata\":{\"Channel\":\"1\",\"MessageNumber\":1,\"MessageTime\":\"2024-04-28T22:20:56.502097+02:00\",\"MessageType\":4},\"Message\":{}}";

    @Test
    public void rocketUpdateModelSchemaShouldCorrectlyDeserializeInputModel(){
        //Arrange
        var deserializationSchema = RocketUpdateModel.getSchema();

        byte[] payloadBytes = samplePaylod.getBytes(StandardCharsets.UTF_8);
        //Act

        try {
            deserializationSchema.open(new InitializationContextMock());

            var result = deserializationSchema.deserialize(payloadBytes);
            //Assert
            Assert.assertNotNull(result);
            Assert.assertNotNull(result.getMessage());

            var metadata = result.getMetadata();
            Assert.assertNotNull(metadata.getChannel());
            Assert.assertEquals(metadata.getChannel(), "1");
            Assert.assertEquals(metadata.getMessageNumber(), 1);
            Assert.assertNotNull(metadata.getMessageType());
            Assert.assertEquals(metadata.getMessageType(), "4");
            Assert.assertNotNull(metadata.getMessageTime());
        } catch (IOException e) {
            Assert.fail("Failure faced during model deserialization");
        }

        //Assert
    }
}
