
using MassTransit;
using RocketGateway.Extensions;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Rockets.Core.Services.Interfaces;
using RocketGateway.Features.Rockets.Framework.ExternalModels.Inbound.Events;
using RocketGateway.Features.Rockets.Framework.ExternalModels.Outbound;
using RocketGateway.Features.Shared.Mapping.Interfaces;
using RocketGateway.Features.Shared.Models;


namespace RocketGateway.Features.Rockets.Framework.Messaging.Consumers;

public class RocketChangeEventBatchConsumer: IConsumer<BatchedModificationEvent>
{
    private readonly IMapper<RocketChangeEvent, OperationResult<RocketChangeCoreEvent, ErrorModel>> mapper;
    private readonly IServiceProvider serviceProvider;

    public RocketChangeEventBatchConsumer(
        IMapper<RocketChangeEvent, OperationResult<RocketChangeCoreEvent, ErrorModel>> mapper,
        IServiceProvider serviceProvider
        )
    {
        this.mapper = mapper;
        this.serviceProvider = serviceProvider;
    }

    public async Task Consume(ConsumeContext<BatchedModificationEvent> context)
    {
        var message = context.Message;
        var aggregatedValidation = OperationResult<RocketChangeEvent, ErrorModel>
            .Travere(message.BatchedModels, mapper.Map);

        var eventHandlingResult = await aggregatedValidation.FlatMapAsync(rocketEvents =>
        {
            var batchedCoreEvents = new BatchedCoreModificationEvent
            {
                ChannelId = message.ChannelId,
                RocketChangeDomainEvents = rocketEvents
            };

            return OperationResult<BatchedCoreModificationEvent, ErrorModel>.FromThrowableOperation(
                () => serviceProvider.GetRequiredService<IRocketsService>(),
                _ => new ErrorModel
                {
                    ErrorCode = ErrorCodes.InvalidDependencyConfiguration,
                    ErrorDescription = "Missing dependency configuration"
                }).FlatMapAsync(service => service.ProcessFilteredEvent(batchedCoreEvents)
            );
        });

        if (!eventHandlingResult.IsSuccess)
        {
            ILogger<RocketChangeEventBatchConsumer> rocketChangeEventLogger = serviceProvider
                .GetRequiredService<ILogger<RocketChangeEventBatchConsumer>>();
            //Maybe DLQ will be sensible there?
            rocketChangeEventLogger.LogError(
                "Failed to process batched filtered message, error occured: {ErrorModel}",
                eventHandlingResult.ErrorModel
                );
        }
    }
}