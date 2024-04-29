using RocketGateway.Features.Rockets.Core.Models.Events;

namespace RocketGateway.Features.Rockets.Framework.Messaging.Producers;

public interface IRocketChangeEventProducer
{
    Task ProduceAsync(RocketChangeCoreEvent rocket);
}