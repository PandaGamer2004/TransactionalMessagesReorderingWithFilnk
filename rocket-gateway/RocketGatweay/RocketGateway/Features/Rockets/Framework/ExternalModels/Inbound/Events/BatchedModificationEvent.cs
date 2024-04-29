namespace RocketGateway.Features.Rockets.Framework.ExternalModels.Inbound.Events;

public class BatchedModificationEvent
{
    public string ChannelId { get; set; }

    public IEnumerable<RocketChangeEvent> BatchedModels { get; set; }
}