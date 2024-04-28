using System.Text.Json;
using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Serializer;
using RocketGateway.Configurations;
using RocketGateway.Exceptions;
using RocketGateway.Messaging.Producers;

namespace RocketGateway.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddKafka(
        this WebApplicationBuilder builder)
    {
        KafkaConfigurationShared kafkaConfigurationShared = builder.Configuration
                                                                .GetSection(KafkaConfigurationShared.ConfigurationKey)
                                                                .Get<KafkaConfigurationShared>()
                                                            ?? throw new ConfigurationException(
                                                                $"Failed to bind configuration on {KafkaConfigurationShared.ConfigurationKey}"
                                                            );

        KafkaProducerConfiguration kafkaProducerConfiguration = builder.Configuration
                                                                    .GetSection(KafkaProducerConfiguration.ConfigurationKey)
                                                                    .Get<KafkaProducerConfiguration>()
                                                                ?? throw new ConfigurationException(
                                                                    $"Failed to bind configuration on {KafkaConfigurationShared.ConfigurationKey}"
                                                                );

        builder.Services
            .AddKafka(kafka =>
                kafka.
                    UseMicrosoftLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { kafkaConfigurationShared.BootstrapServer })
                            .AddProducer(
                                Producers.RocketMutationEventProducer,
                                producer => 
                                    producer
                                        .AddMiddlewares(middleware => 
                                            middleware.AddSerializer<NewtonsoftJsonSerializer>()
                                            )
                                        .WithProducerConfig(new ProducerConfig
                                        {
                                            EnableIdempotence = true,
                                            CompressionType = CompressionType.Gzip,
                                            Acks = kafkaProducerConfiguration.Acks,
                                        })
                                        .DefaultTopic(kafkaProducerConfiguration.Topic)
                            )
                    )
            );

        return builder;
    }
}