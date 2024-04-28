using RocketGateway.Features.Rockets.Models;
using RocketGateway.Features.Shared.Models;

namespace RocketGateway.Features.Rockets.Core.Models.Messages;

public class RocketExplodedMessage: IRocketMessage
{
    public string Reason { get; set; }
}