namespace RocketGateway.Features.Rockets.Framework.ExternalModels.Outbound;

public class ErrorCodes
{
    public const int FailedToMapEventType = 1023;

    public const int InvalidPayloadForEventType = 1024;

    public const int FailedToProcessEvent = 1025;

    public const int InvalidDependencyConfiguration = 1026;

    public const int FailureToAccessRocketById = 1028;

    public const int FailedToLoadRockets = 1027;
}