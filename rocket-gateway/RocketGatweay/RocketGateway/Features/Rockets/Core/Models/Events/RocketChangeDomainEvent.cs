
using RocketGateway.Features.Rockets.Models;

namespace RocketGateway.Features.Rockets.Core.Models.Events;

public class RocketChangeDomainEvent
{
    public RocketEventDomainMetadata Metadata { get; set; }

    public IRocketMessage Message { get; set; }
}