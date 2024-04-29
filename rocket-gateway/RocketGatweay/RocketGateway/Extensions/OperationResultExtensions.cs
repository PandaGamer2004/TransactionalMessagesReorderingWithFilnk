using RocketGateway.Features.Shared.Models;

namespace RocketGateway.Extensions;

public static class OperationResultExtensions
{
    public static OperationResult<TResult, TError> Map<TSuccess, TError, TResult>(
        this OperationResult<TSuccess, TError> operationResult,
        Func<TSuccess, TResult> projector)
    {
        if (operationResult.IsSuccess)
        {
            return OperationResult<TResult, TError>.CreateSuccess(
                projector(operationResult.SuccessModel)
            );
        }
        return OperationResult<TResult, TError>.CreateError(
            operationResult.ErrorModel
            );
    }

    public static OperationResult<TResult, TError> FlatMap<TSuccess, TError, TResult>(
        this OperationResult<TSuccess, TError> operationResult,
        Func<TSuccess, OperationResult<TResult, TError>> projector)
    {
        if (operationResult.IsSuccess)
        {
            return projector(operationResult.SuccessModel);
        }
        return OperationResult<TResult, TError>.CreateError(
            operationResult.ErrorModel
        );
    }
    
    
}