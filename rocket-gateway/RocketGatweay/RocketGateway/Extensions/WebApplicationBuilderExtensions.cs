using MassTransit;
using MassTransit.KafkaIntegration.Serializers;
using Microsoft.EntityFrameworkCore;
using RocketGateway.Configurations;
using RocketGateway.DbContext;
using RocketGateway.Features.Rockets.Core.Models.Events;
using RocketGateway.Features.Rockets.Framework.ExternalModels.Inbound.Events;
using RocketGateway.Features.Rockets.Framework.Messaging.Consumers;
using ConfigurationException = RocketGateway.Exceptions.ConfigurationException;


namespace RocketGateway.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddPostgres(this WebApplicationBuilder builder)
    {
        string postgresConnectionString = builder.Configuration.GetConnectionString("Postgres")
                                                ?? throw new ConfigurationException(
                                                    "Postgres configuration string have to be supplied"
                                                    );
        builder.Services.AddDbContext<RocketsDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(postgresConnectionString);
        });
        return builder;
    }
    
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
                rider.AddProducer<Guid, RocketChangeCoreEvent>(
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