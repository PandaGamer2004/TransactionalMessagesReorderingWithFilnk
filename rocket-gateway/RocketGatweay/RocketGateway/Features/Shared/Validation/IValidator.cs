using RocketGateway.Features.Shared.Models;

namespace RocketGateway.Features.Shared.Validation;

public interface IValidator<TModel>
{
    public ValidationResult<string> Validate(TModel model);
}