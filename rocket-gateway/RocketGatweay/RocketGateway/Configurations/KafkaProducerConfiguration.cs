using KafkaFlow;
using Acks = Confluent.Kafka.Acks;

namespace RocketGateway.Configurations;

public class KafkaProducerConfiguration
{
    public const string ConfigurationKey = "KafkaConfiguration:Producer";
    
    public Acks? Acks { get; set; }

    public string Topic { get; set; }
}