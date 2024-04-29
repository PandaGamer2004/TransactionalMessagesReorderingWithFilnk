package unit.mappers;


import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.core.JsonProcessingException;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.JsonNode;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.ObjectMapper;
import org.daniil.models.RocketUpdateModel;
import org.daniil.models.RocketUpdateModelMetadata;
import org.daniil.projectors.ToFlatModelMapper;
import org.junit.Assert;
import org.junit.Test;

import java.time.OffsetDateTime;

public class ToFlatModelMapperTests {

    @Test
    public void ToFlatModelMapperShouldPreserveEntityStructure() throws JsonProcessingException {
        //Arrange
        var mapper = new ToFlatModelMapper();
        var model = new RocketUpdateModel();
        JsonNode messageNode = new ObjectMapper()
                .readTree( "{\"by\": \"3000\"}");
        var sampleMetadata = new RocketUpdateModelMetadata();
        sampleMetadata.setMessageType("a");
        sampleMetadata.setMessageTime(OffsetDateTime.now());
        sampleMetadata.setMessageNumber(1);
        sampleMetadata.setChannel("a");

        //Act
        model.setMessage(messageNode);
        model.setMetadata(sampleMetadata);

        var flatModel = mapper.map(model);

        //Assert
        Assert.assertNotNull(flatModel);
        Assert.assertEquals(flatModel.getMessage(), messageNode);
        Assert.assertEquals(flatModel.getMessageNumber(), sampleMetadata.getMessageNumber());
        Assert.assertEquals(flatModel.getMessageTime(), sampleMetadata.getMessageTime());
        Assert.assertEquals(flatModel.getChannel(), sampleMetadata.getChannel());
        Assert.assertEquals(flatModel.getMessageType(), sampleMetadata.getMessageType());
    }
}
