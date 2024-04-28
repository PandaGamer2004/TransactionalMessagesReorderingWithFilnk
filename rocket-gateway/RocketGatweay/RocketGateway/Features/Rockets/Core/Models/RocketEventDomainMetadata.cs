using RocketGateway.Features.Rockets.Models;

namespace RocketGateway.Features.Rockets.Core.Models;

public class RocketEventDomainMetadata
{
    public string Channel { get; set; }

    //In the Channel
    public int MessageNumber { get; set; }

    public DateTimeOffset MessageTime { get; set; }

    public RocketMessageType MessageType { get; set; }

    public static RocketEventDomainMetadata FromMetadata(RocketEventMetadata metadata, RocketMessageType messageType)
        => new()
        {
            Channel = metadata.Channel,
            MessageNumber = metadata.MessageNumber,
            MessageTime = metadata.MessageTime,
            MessageType = messageType
        };
}