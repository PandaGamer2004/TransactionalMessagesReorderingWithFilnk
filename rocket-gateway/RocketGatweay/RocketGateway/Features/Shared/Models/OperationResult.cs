namespace RocketGateway.Features.Shared.Models;

public struct OperationResult<TSuccessModel, TErrorModel>
{
    public TSuccessModel SuccessModel { get; private set; }

    public TErrorModel ErrorModel { get; private set; }

    public bool IsSuccess { get; private set; }

    public static OperationResult<TSuccessModel, TErrorModel> CreateSuccess(TSuccessModel successModel)
        => new OperationResult<TSuccessModel, TErrorModel>
        {
            IsSuccess = true,
            SuccessModel = successModel
        };

    public static OperationResult<TSuccessModel, TErrorModel> CreateError(TErrorModel errorModel)
        => new OperationResult<TSuccessModel, TErrorModel>
        {
            IsSuccess = false,
            ErrorModel = errorModel
        };

    public static OperationResult<TSuccessModel, TErrorModel> Unit(TSuccessModel op)
        => OperationResult<TSuccessModel, TErrorModel>.CreateSuccess(op);
}
