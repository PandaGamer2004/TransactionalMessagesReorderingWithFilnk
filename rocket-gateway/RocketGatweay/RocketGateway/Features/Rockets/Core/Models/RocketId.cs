namespace RocketGateway.Features.Rockets.Core.Models;

public class RocketId
{
    public Guid Value { get; private set; }

    public static RocketId FromValue(Guid rocketId)
        => new RocketId
        {
            IsInitialized = true,
            Value = rocketId
        };
    public bool IsInitialized { get; private set; }
}