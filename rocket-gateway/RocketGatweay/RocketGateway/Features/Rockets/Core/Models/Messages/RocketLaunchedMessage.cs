using RocketGateway.Features.Rockets.Models;

namespace RocketGateway.Features.Rockets.Core.Models.Messages;

public class RocketLaunchedMessage: IRocketMessage
{
    public string Type { get; set; }

    public int LaunchSpeed { get; set; }

    public string Mission { get; set; }
    
}