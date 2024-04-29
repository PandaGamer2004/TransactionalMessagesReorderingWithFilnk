using RocketGateway.Features.Rockets.Core.Models.Interfaces;

namespace RocketGateway.Features.Rockets.Core.Models.Messages;

public class RocketExplodedMessage: IRocketMessage
{
    public string Reason { get; set; }
}