using System.Text.Json;
using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Serializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RocketGateway.Configurations;
using RocketGateway.Exceptions;
using RocketGateway.Features.Rockets.Framework.Messaging.Consumers;
using RocketGateway.Features.Rockets.Framework.Messaging.Producers;
using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

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

        var kafkaSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        
        builder.Services
            .AddKafka(kafka =>
                kafka.
                    UseMicrosoftLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { kafkaConfigurationShared.BootstrapServer })
                            .AddConsumer(consumer => 
                                consumer
                                    .WithAutoOffsetReset(kafkaConsumerConfiguration.AutoOffsetReset)
                                    .Topic(kafkaConsumerConfiguration.Topic)
                                    .WithGroupId(kafkaConsumerConfiguration.GroupId)
                                    .WithWorkersCount(kafkaConsumerConfiguration.Workers)
                                    .WithBufferSize(kafkaConsumerConfiguration.BufferSize)
                                    .AddMiddlewares(m => 
                                        m.AddDeserializer<NewtonsoftJsonDeserializer>(
                                            _ => new NewtonsoftJsonDeserializer(kafkaSerializerSettings))
                                         .AddTypedHandlers(h => h.AddHandler<RocketChangeEventBatchConsumer>())
                                        )
                                    
                                )
                            .AddProducer(
                                Producers.RocketMutationEventProducer,
                                producer => 
                                    producer
                                        .AddMiddlewares(middleware => 
                                            middleware.AddSerializer<NewtonsoftJsonSerializer>(_ => new NewtonsoftJsonSerializer(kafkaSerializerSettings)))
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