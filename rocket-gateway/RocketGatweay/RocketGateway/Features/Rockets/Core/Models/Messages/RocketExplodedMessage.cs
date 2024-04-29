using RocketGateway.Features.Rockets.Models;

namespace RocketGateway.Features.Rockets.Core.Models.Messages;

public class RocketExplodedMessage: IRocketMessage
{
    public string Reason { get; set; }
}