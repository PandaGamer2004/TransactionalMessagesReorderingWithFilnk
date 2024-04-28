namespace RocketGateway.Features.Shared.Results;

//Implemented as struct to reduce multiple heavy allocations
public class VoidResult
{
    public static VoidResult Instance = new();
}