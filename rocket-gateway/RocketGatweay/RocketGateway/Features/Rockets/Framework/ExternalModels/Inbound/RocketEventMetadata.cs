namespace RocketGateway.Features.Rockets.Framework.ExternalModels.Inbound;

public class RocketEventMetadata
{
    public string Channel { get; set; }

    //In the Channel
    public int MessageNumber { get; set; }

    public DateTimeOffset MessageTime { get; set; }

    public string MessageType { get; set; }
}