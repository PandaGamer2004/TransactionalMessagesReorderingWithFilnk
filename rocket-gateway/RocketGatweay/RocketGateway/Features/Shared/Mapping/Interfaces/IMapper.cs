namespace RocketGateway.Features.Shared.Mapping.Interfaces;

public interface IMapper<TInput, TResult>
{
    TResult Map(TInput input);
}