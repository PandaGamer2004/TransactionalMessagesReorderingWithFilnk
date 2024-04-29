namespace RocketGateway.Features.Rockets.Core.Models.Events;

public class BatchedCoreModificationEvent
{
    //Basically our aggregate id
    public Guid ChannelId { get; set; }

    public IEnumerable<RocketChangeCoreEvent> RocketChangeDomainEvents { get; set; }
} 