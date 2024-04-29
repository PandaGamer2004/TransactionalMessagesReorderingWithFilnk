using KafkaFlow.Producers;
using RocketGateway.Features.Rockets.Core.Models.Events;

namespace RocketGateway.Features.Rockets.Framework.Messaging.Producers;

public class RocketChangeEventProducer: IRocketChangeEventProducer
{
    private readonly IProducerAccessor producerAccessor;

    public RocketChangeEventProducer(IProducerAccessor producerAccessor)
    {
        this.producerAccessor = producerAccessor;
    }


    public Task ProduceAsync(
        RocketChangeCoreEvent rocket
    )
    {
        var producer = producerAccessor.GetProducer(Features.Rockets.Framework.Messaging.Producers.Producers.RocketMutationEventProducer);
        return producer.ProduceAsync(
            rocket.Metadata.Channel,
            rocket);
    }
}