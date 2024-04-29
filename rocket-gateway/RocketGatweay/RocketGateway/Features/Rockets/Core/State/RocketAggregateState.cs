using RocketGateway.Features.Rockets.Core.Models;

namespace RocketGateway.Features.Rockets.Core.State;

public class RocketAggregateState
{
    public RocketId Id { get;  set; }

    public string Type { get;  set; }

    public int LaunchSpeed { get;  set; }

    public string Mission { get;  set; }

    public string ExplodedReason { get;  set; }

    public bool WasExploded { get;  set; }
}