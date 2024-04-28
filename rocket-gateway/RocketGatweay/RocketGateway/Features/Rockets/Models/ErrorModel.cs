namespace RocketGateway.Features.Rockets.Models;

public class ErrorModel
{
    public required int ErrorCode { get; set; }

    public required string ErrorDescription { get; set; }
}