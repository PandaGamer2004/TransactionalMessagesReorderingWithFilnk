using RocketGateway.Features.Rockets.Core.Models.Interfaces;

namespace RocketGateway.Features.Rockets.Core.Models.Messages;

//Will serve both for increased and decreased
public class RocketSpeedChangeMessage: IRocketMessage
{
    public int By { get; set; }
}