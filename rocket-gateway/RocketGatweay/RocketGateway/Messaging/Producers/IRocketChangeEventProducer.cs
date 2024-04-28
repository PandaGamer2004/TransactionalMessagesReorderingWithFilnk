using RocketGateway.Features.Rockets.Core.Models.Events;

namespace RocketGateway.Messaging.Producers;

public interface IRocketChangeEventProducer
{
    Task ProduceAsync(RocketChangeDomainEvent rocket);
}