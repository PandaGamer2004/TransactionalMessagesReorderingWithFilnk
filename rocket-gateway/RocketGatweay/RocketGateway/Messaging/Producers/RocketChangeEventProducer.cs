using KafkaFlow;
using RocketGateway.Features.Rockets.Core.Models.Events;

namespace RocketGateway.Messaging.Producers;

public class RocketChangeEventProducer: IRocketChangeEventProducer
{
    private readonly IMessageProducer<RocketChangeDomainEvent> producer;

    public RocketChangeEventProducer(IMessageProducer<RocketChangeDomainEvent> producer)
    {
        this.producer = producer;
    }
    public Task ProduceAsync(
        RocketChangeDomainEvent rocket, 
        CancellationToken ct
        ) 
        => producer.ProduceAsync(
            rocket.Metadata.Channel, 
            rocket,
            ct);
    
}