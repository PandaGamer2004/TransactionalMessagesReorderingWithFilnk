using MassTransit;
using MassTransit.KafkaIntegration.Serializers;
using RocketGateway.Features.Rockets.Core.Models.Events;

namespace RocketGateway.Features.Rockets.Framework.Messaging.Producers;

public class RocketChangeEventProducer: IRocketChangeEventProducer
{
    private readonly ITopicProducer<string, RocketChangeCoreEvent> rocketChangeEventProducer;

    public RocketChangeEventProducer(ITopicProducer<string, RocketChangeCoreEvent> rocketChangeEventProducer)
    {
        this.rocketChangeEventProducer = rocketChangeEventProducer;
    }


    public Task ProduceAsync(
        RocketChangeCoreEvent rocket
    )
    {
        var configuredPipe = Pipe.Execute<KafkaSendContext<string, RocketChangeCoreEvent>>(context =>
        {
            context.KeySerializer
                = new MassTransitAsyncJsonSerializer<string>();
            context.ValueSerializer =
                new MassTransitAsyncJsonSerializer<RocketChangeCoreEvent>();
        });

        return this.rocketChangeEventProducer.Produce(rocket.Metadata.Channel, rocket, configuredPipe);
    }
}