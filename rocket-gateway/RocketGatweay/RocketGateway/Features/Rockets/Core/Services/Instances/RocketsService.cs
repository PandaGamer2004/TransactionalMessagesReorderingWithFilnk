using RocketGateway.Features.Rockets.Core.Models;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Rockets.Core.Services.Interfaces;
using RocketGateway.Features.Rockets.Framework.Messaging.Producers;
using RocketGateway.Features.Shared.Models;
using RocketGateway.Features.Shared.Results;
using RocketGateway.Features.Shared.Validation;

namespace RocketGateway.Features.Rockets.Core.Services.Instances;

public class RocketsService: IRocketsService
{
    private readonly IRocketChangeEventProducer _rocketChangeEventProducer;
    private readonly IValidator<RocketChangeCoreEvent> changeValidator;

    public RocketsService(
        IRocketChangeEventProducer rocketChangeEventProducer, 
        IValidator<RocketChangeCoreEvent> changeValidator
        )
    {
        _rocketChangeEventProducer = rocketChangeEventProducer;
        this.changeValidator = changeValidator;
    }

    public async Task<OperationResult<VoidResult, string>> ProcessUnfilteredEvent(
        RocketChangeCoreEvent rocketChangeCoreEvent, 
        CancellationToken ct = default
        )
    {
        var validationResult = changeValidator.Validate(rocketChangeCoreEvent);
        if (validationResult.IsValid)
        {
            try
            {
                await _rocketChangeEventProducer.ProduceAsync(
                    rocketChangeCoreEvent);
                return OperationResult<VoidResult, string>.CreateSuccess(
                    VoidResult.Instance
                );
            }
            catch (Exception ex)
            {
                return OperationResult<VoidResult, string>.CreateError(
                    "Failed to process rocket change. Possible middleware outage."
                );
            }
        }

        return OperationResult<VoidResult, string>.CreateError(
            $"Failed validation. Error: {validationResult.ErrorModel}"
        );
    }

    public Task<OperationResult<VoidResult, string>> ProcessFilteredEventBatch(
        FilteredEventsBatch eventsBatch, 
        CancellationToken ct = default
        )
    {
        throw new NotImplementedException();
    }
}