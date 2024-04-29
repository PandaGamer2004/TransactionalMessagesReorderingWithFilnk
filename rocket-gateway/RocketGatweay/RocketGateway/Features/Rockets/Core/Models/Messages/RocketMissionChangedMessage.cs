using RocketGateway.Features.Rockets.Core.Models.Interfaces;
using RocketGateway.Features.Shared.Models;

namespace RocketGateway.Features.Rockets.Core.Models.Messages;

public class RocketMissionChangedMessage: IRocketMessage
{
    public string NewMission { get; set; }
    
    
}