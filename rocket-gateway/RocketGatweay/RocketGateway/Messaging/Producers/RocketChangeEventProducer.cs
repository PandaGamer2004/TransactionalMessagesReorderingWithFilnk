using KafkaFlow;
using KafkaFlow.Producers;
using RocketGateway.Features.Rockets.Core.Models.Events;

namespace RocketGateway.Messaging.Producers;

public class RocketChangeEventProducer: IRocketChangeEventProducer
{
    private readonly IProducerAccessor producerAccessor;

    public RocketChangeEventProducer(IProducerAccessor producerAccessor)
    {
        this.producerAccessor = producerAccessor;
    }


    public Task ProduceAsync(
        RocketChangeDomainEvent rocket,
        CancellationToken ct
    )
    {
        var producer = producerAccessor.GetProducer(Producers.RocketMutationEventProducer);
        return producer.ProduceAsync(
            rocket.Metadata.Channel,
            rocket,
            ct);
    }
}