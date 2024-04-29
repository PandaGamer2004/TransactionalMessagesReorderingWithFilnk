namespace RocketGateway.Features.Rockets.Framework.ExternalModels.Inbound.Events;

public class BatchedModificationEvent
{
    public Guid ChannelId { get; set; }

    public IEnumerable<RocketChangeEvent> BatchedModels { get; set; }
}