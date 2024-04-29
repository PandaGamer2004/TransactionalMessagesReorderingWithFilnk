using KafkaFlow;
using RocketGateway.Features.Rockets.Framework.ExternalModels.Inbound.Events;

namespace RocketGateway.Features.Rockets.Framework.Messaging.Consumers;

public class RocketChangeEventBatchConsumer: IMessageHandler<BatchedModificationEvent>
{
    public Task Handle(IMessageContext context, BatchedModificationEvent message)
    {
        context.ap
    }
}