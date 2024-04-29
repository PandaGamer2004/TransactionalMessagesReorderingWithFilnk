using RocketGateway.Features.Rockets.Core.Models.Events;

namespace RocketGateway.Features.Rockets.Core.Models;

public class FilteredEventsBatch
{
    public string ChannelId { get; set; }
    
    public IEnumerable<RocketChangeCoreEvent> OrderedAndDeduplicatedEvents { get; set; }
}