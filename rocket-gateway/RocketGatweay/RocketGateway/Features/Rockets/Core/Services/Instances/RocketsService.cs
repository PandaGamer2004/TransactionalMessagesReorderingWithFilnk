using RocketGateway.Features.Rockets.Core.Models;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Rockets.Core.Services.Interfaces;
using RocketGateway.Features.Shared.Models;
using RocketGateway.Features.Shared.Results;
using RocketGateway.Features.Shared.Validation;
using RocketGateway.Messaging.Producers;

namespace RocketGateway.Features.Rockets.Core.Services.Instances;

public class RocketsService: IRocketsService
{
    private readonly IRocketChangeEventProducer _rocketChangeEventProducer;
    private readonly IValidator<RocketChangeDomainEvent> changeValidator;

    public RocketsService(
        IRocketChangeEventProducer rocketChangeEventProducer, 
        IValidator<RocketChangeDomainEvent> changeValidator
        )
    {
        _rocketChangeEventProducer = rocketChangeEventProducer;
        this.changeValidator = changeValidator;
    }

    public async Task<OperationResult<VoidResult, string>> ProcessUnfilteredEvent(
        RocketChangeDomainEvent rocketChangeDomainEvent, 
        CancellationToken ct = default
        )
    {
        var validationResult = changeValidator.Validate(rocketChangeDomainEvent);
        if (validationResult.IsValid)
        {
            try
            {
                await _rocketChangeEventProducer.ProduceAsync(
                    rocketChangeDomainEvent,
                    ct);
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