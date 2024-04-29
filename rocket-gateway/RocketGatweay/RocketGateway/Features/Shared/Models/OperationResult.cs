using RocketGateway.Extensions;

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

    
    public static OperationResult<IEnumerable<TSuccessModel>, TErrorModel> Traverse<TInput, TSuccessModel, TErrorModel>(
        IEnumerable<TInput> inputs,
        Func<TInput, OperationResult<TSuccessModel, TErrorModel>> projector
    )
    {
        var aggregator = OperationResult<LinkedList<TSuccessModel>, TErrorModel>
            .Unit(new LinkedList<TSuccessModel>());
        
        return inputs.Select(projector).Aggregate(aggregator, 
            (agg, it) 
                => agg.FlatMap(list => it.Map(list.Prepend)
                )
        ).Map(it => it.AsEnumerable());
    }

    public static OperationResult<TResult, TError> FromThrowableOperation<TResult, TError>(
        Func<TResult> projector,
        Func<Exception, TError> errorProjector)
    {
        try
        {
            TResult result = projector();
            return OperationResult<TResult, TError>
                .CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return OperationResult<TResult, TError>
                .CreateError(errorProjector(ex));
        }
    }
}
