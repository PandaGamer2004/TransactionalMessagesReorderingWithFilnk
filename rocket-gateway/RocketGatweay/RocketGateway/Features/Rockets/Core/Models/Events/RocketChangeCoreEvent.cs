
using RocketGateway.Features.Rockets.Core.Models.Interfaces;

namespace RocketGateway.Features.Rockets.Core.Models.Events;

public class RocketChangeCoreEvent
{
    public RocketEventDomainMetadata Metadata { get; set; }

    public IRocketMessage Message { get; set; }
}