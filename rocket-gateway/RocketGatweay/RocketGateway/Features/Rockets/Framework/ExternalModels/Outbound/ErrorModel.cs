namespace RocketGateway.Features.Rockets.Framework.ExternalModels.Outbound;

public class ErrorModel
{
    public required int ErrorCode { get; set; }

    public required string ErrorDescription { get; set; }
}