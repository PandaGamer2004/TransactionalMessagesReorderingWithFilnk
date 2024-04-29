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

    
    public static Task<OperationResult<TResult, TError>> FlatMapAsync<TInput, TError, TResult>(
        this OperationResult<TInput, TError> operationResult,
        Func<TInput, Task<OperationResult<TResult, TError>>> projector)
    {
        if (operationResult.IsSuccess)
        {
            var successModel = operationResult.SuccessModel;
            return projector(successModel)
                .ContinueWith(
                    t => t.Result,
                    TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.NotOnFaulted
                );
        }

        return Task.FromResult(OperationResult<TResult, TError>.CreateError(
            operationResult.ErrorModel
        ));
    }


    public static Task<OperationResult<TResult, TErrorResult>> ApplyErrorProjectionAsync<TResult, TError, TErrorResult>(
        this Task<OperationResult<TResult, TError>> operationResultTask,
        Func<TError, TErrorResult> errorProjector) 
        => operationResultTask.ContinueWith(
            t => t.Result.ApplyErrorProjection(errorProjector),
            TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.NotOnFaulted);

    
    public static OperationResult<TResult, TErrorResult> ApplyErrorProjection<TResult, TError, TErrorResult>(
        this OperationResult<TResult, TError> operationResult,
        Func<TError, TErrorResult> errorProjector)
    {
        if (!operationResult.IsSuccess)
        {
            return OperationResult<TResult, TErrorResult>.CreateError(
                errorProjector(operationResult.ErrorModel)
                );
        }

        return OperationResult<TResult, TErrorResult>.CreateSuccess(
            operationResult.SuccessModel
            );
    }
    
    
}