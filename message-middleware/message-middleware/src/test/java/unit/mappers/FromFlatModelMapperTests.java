package unit.mappers;

import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.core.JsonProcessingException;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.JsonNode;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.ObjectMapper;
import org.daniil.models.RocketUpdateFlatModel;
import org.daniil.projectors.FromFlatModelMapper;
import org.junit.Assert;
import org.junit.Test;

import java.time.OffsetDateTime;

public class FromFlatModelMapperTests {

    @Test
    public void ToFlatModelMapperShouldPreserveEntityStructure() throws JsonProcessingException {
        //Arrange
        var mapper = new FromFlatModelMapper();
        var flatModel = new RocketUpdateFlatModel();
        JsonNode messageNode = new ObjectMapper()
                .readTree( "{\"by\": \"3000\"}");
        flatModel.setMessageType("a");
        flatModel.setMessageTime(OffsetDateTime.now());
        flatModel.setMessageNumber(1);
        flatModel.setChannel("a");

        //Act
        flatModel.setMessage(messageNode);


        var projectedModel = mapper.map(flatModel);

        //Assert
        Assert.assertNotNull(projectedModel);
        var messageMetadata = projectedModel.getMetadata();

        Assert.assertEquals(flatModel.getMessage(), projectedModel.getMessage());
        Assert.assertEquals(flatModel.getMessageNumber(), messageMetadata.getMessageNumber());
        Assert.assertEquals(flatModel.getMessageTime(), messageMetadata.getMessageTime());
        Assert.assertEquals(flatModel.getChannel(), messageMetadata.getChannel());
        Assert.assertEquals(flatModel.getMessageType(), messageMetadata.getMessageType());
    }
}
