using MassTransit;
using MassTransit.KafkaIntegration.Serializers;
using RocketGateway.Configurations;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Rockets.Framework.ExternalModels.Inbound.Events;
using RocketGateway.Features.Rockets.Framework.Messaging.Consumers;


namespace RocketGateway.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddKafka(
        this WebApplicationBuilder builder)
    {

        KafkaConfigurationShared kafkaConfigurationShared = builder.Configuration
            .GetModelOrThrow<KafkaConfigurationShared>(KafkaConfigurationShared.ConfigurationKey);

        KafkaProducerConfiguration kafkaProducerConfiguration =
            builder.Configuration.GetModelOrThrow<KafkaProducerConfiguration>(
                KafkaProducerConfiguration.ConfigurationKey);

        KafkaConsumerConfiguration kafkaConsumerConfiguration =
            builder.Configuration.GetModelOrThrow<KafkaConsumerConfiguration>(
                KafkaConsumerConfiguration.ConfigurationSection
            );


        builder.Services.AddMassTransit(o =>
        {
            o.UsingInMemory();
            o.AddRider(rider =>
            {

                rider.AddConsumer<RocketChangeEventBatchConsumer>();
                rider.AddProducer<string, RocketChangeCoreEvent>(
                    kafkaProducerConfiguration.Topic,
                    (ctx, config) =>
                    {
                        config.EnableIdempotence = true;
                    });

                rider.UsingKafka((context, k) =>
                {
                    k.Acks = kafkaProducerConfiguration.Acks;

                    k.Host(kafkaConfigurationShared.BootstrapServer);

                    k.TopicEndpoint<BatchedModificationEvent>(kafkaConsumerConfiguration.Topic,
                        kafkaConsumerConfiguration.GroupId,
                        e =>
                        {
                            e.SetValueDeserializer(
                                new MassTransitJsonDeserializer<BatchedModificationEvent>()
                            );
                            e.AutoOffsetReset = kafkaConsumerConfiguration.AutoOffsetReset;
                            e.ConfigureConsumer<RocketChangeEventBatchConsumer>(context);
                        });
                });
            });
        });
        

        return builder;
    }
}