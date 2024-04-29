using RocketGateway.Extensions;
using RocketGateway.Features.Rockets.Core.Aggregates;
using RocketGateway.Features.Rockets.Core.Models;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Rockets.Core.Repositories;
using RocketGateway.Features.Rockets.Core.Services.Interfaces;
using RocketGateway.Features.Rockets.Framework.Messaging.Producers;
using RocketGateway.Features.Shared.Models;
using RocketGateway.Features.Shared.Results;
using RocketGateway.Features.Shared.Validation;

namespace RocketGateway.Features.Rockets.Core.Services.Instances;

public class RocketsService: IRocketsService
{
    private readonly IRocketChangeEventProducer rocketChangeEventProducer;
    private readonly IValidator<RocketChangeCoreEvent> changeValidator;
    private readonly IRocketRepository rocketRepository;

    public RocketsService(
        IRocketChangeEventProducer rocketChangeEventProducer, 
        IValidator<RocketChangeCoreEvent> changeValidator, IRocketRepository rocketRepository)
    {
        this.rocketChangeEventProducer = rocketChangeEventProducer;
        this.changeValidator = changeValidator;
        this.rocketRepository = rocketRepository;
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
                await this.rocketChangeEventProducer.ProduceAsync(
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

    public async Task<OperationResult<VoidResult, string>> ProcessFilteredEvent(
        BatchedCoreModificationEvent batchedCoreModificationEvent, 
        CancellationToken ct = default
        )
    {
        var loadedRocket = await this.rocketRepository.LoadRocketBy(
            RocketId.FromValue(batchedCoreModificationEvent.ChannelId)
            );

        return await loadedRocket.FlatMap(rocketAggregate =>
        {
            var changeDomainEvents
                = batchedCoreModificationEvent.RocketChangeDomainEvents
                    .ToArray();
            //If not domain events was received in batch we can successfully finish the operation 
            if (!changeDomainEvents.Any())
            {
                return OperationResult<RocketAggregate, String>
                    .CreateError("No events supplied to process");
            }

            return OperationResult<VoidResult, string>.Traverse(
                batchedCoreModificationEvent.RocketChangeDomainEvents,
                rocketAggregate.Apply
            ).Map(_ => rocketAggregate);
        }).FlatMapAsync(
            patchedRocket => rocketRepository.Store(patchedRocket, ct)
        );
    }

    public Task<OperationResult<RocketAggregate, string>> GetRocketById(RocketId rocketId,
        CancellationToken ct = default)
        => this.rocketRepository.LoadRocketBy(rocketId, ct);


    public Task<OperationResult<IEnumerable<RocketAggregate>, string>> GetRockets(CancellationToken ct = default)
        => this.rocketRepository.LoadRockets();

}