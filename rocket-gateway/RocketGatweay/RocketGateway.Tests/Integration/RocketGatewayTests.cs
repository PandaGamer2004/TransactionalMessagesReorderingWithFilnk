using System.Net;
using System.Net.Http.Json;
using RocketGateway.Features.Rockets.Core.Models.Messages;
using RocketGateway.Tests.Integration.Models;
using RocketGateway.Tests.SUT;
using RocketLaunchedMessage = RocketGateway.Tests.Integration.Models.RocketLaunchedMessage;

namespace RocketGateway.Tests.Integration;


public class RocketGatewayTests: IClassFixture<CustomWebApplicationFactory>
{
    private List<RocketEventModel> ValidModels = new List<RocketEventModel>
    {
        new()
        {
            Metadata = RocketMetadata.MakeWithType("RocketLaunched", 1),
            Message = new RocketLaunchedMessage
            {
                Mission = "ARTEMIS",
                Type = "Falcon-9",
                LaunchSpeed = 900,
            }
        },
        new()
        {
            Metadata = RocketMetadata.MakeWithType("RocketMissionChanged", 4),
            Message = new RocketMissionChangedMessage
            {
                NewMission = "SHUTTLE_MIr"
            }
        },
        new()
        {
            Metadata = RocketMetadata.MakeWithType("RocketExploded", 5),
            Message = new RocketExploded
            {
                Reason = "Some reason"
            }
        },
        new()
        {
            Metadata = RocketMetadata.MakeWithType("RocketExploded", 5),
            Message = new RocketExploded
            {
                Reason = "Some reason"
            }
        },
        new()
        {
            Metadata = RocketMetadata.MakeWithType("RocketExploded", 5),
            Message = new RocketExploded
            {
                Reason = "Some reason"
            }
        },
        new()
        {
            Metadata = RocketMetadata.MakeWithType("RocketSpeedIncreased", 2),
            Message = new RocketSpeedIncreased
            {
                By = 3000
            }
        },
        new()
        {
            Metadata = RocketMetadata.MakeWithType("RocketSpeedDecreased", 3),
            Message = new RocketSpeedDecreased
            {
                By = 2500
            }
        },
    };

    private RocketEventModel IvalidKeyModel = new RocketEventModel
    {
        Metadata = RocketMetadata.MakeWithType("invalid", 1),
        Message = new RocketExploded
        {
            Reason = "Some reason"
        }
    };

    private RocketEventModel InvalidMessageModel = new RocketEventModel
    {
        Metadata = RocketMetadata.MakeWithType("RocketExploded", 1),
        Message = new RocketMessageInvalidPayload()
    };
    
    private readonly CustomWebApplicationFactory _webApplicationFactory;
    private const string UpdateEventEndpoint = "/api/Rocket/patch";
    
    public RocketGatewayTests(CustomWebApplicationFactory webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }
    [Fact]
    public async Task   RocketGatewayShouldAcceptValidMessagePayloads()
    {
        //Arrange
        var client = _webApplicationFactory.CreateClient();
        
        //Act
        var performedQueries = ValidModels.Select(
            model => client.PostAsync(
                UpdateEventEndpoint,
                JsonContent.Create(model, model.GetType())
                )
        );
        var result = 
            await Task.WhenAll(performedQueries);

        //Asert
        Assert.True(
            result.All(message => message.StatusCode == HttpStatusCode.OK), 
            "Some of valid models requests have failed"); 
    }

    [Fact]
    public async Task RocketGatewayShouldRejectInvalidMessageType()
    {
        //Arrange
        var client = _webApplicationFactory.CreateClient();
        
        //Act
        var updateResult = await client.PostAsync(
            UpdateEventEndpoint, 
            JsonContent.Create(IvalidKeyModel)
            );
        
        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, updateResult.StatusCode);
    }

    [Fact]
    public async Task RocketGatewayShouldRejectInvalidMessagePayload()
    {
        //Arrange
        var client = _webApplicationFactory.CreateClient();

        //Act
        var updateResult = await client.PostAsync(
            UpdateEventEndpoint, 
            JsonContent.Create(InvalidMessageModel)
        );
        
        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, updateResult.StatusCode);
    }
}