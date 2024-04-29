
using RocketGateway.Features.Rockets.Models;

namespace RocketGateway.Features.Rockets.Core.Models.Events;

public class RocketChangeCoreEvent
{
    public RocketEventDomainMetadata Metadata { get; set; }

    public IRocketMessage Message { get; set; }
}