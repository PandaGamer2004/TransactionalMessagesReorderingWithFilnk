using System.ComponentModel.DataAnnotations;

namespace RocketGateway.Features.Shared.Models;

public class ValidationResult<TErrorModel>
{
    public TErrorModel ErrorModel { get; set; }

    public bool IsValid { get; private set; }

    
    private ValidationResult(bool isValid, TErrorModel errorModel = default)
    {
        IsValid = isValid;
        ErrorModel = errorModel;
    }

    public static ValidationResult<TErrorModel> CreateSuccess()
        => new ValidationResult<TErrorModel>(true);

    public static ValidationResult<TErrorModel> CreateError(TErrorModel errorModel)
        => new ValidationResult<TErrorModel>(false, errorModel);

}