using Confluent.Kafka;

namespace RocketGateway.Configurations;

public class KafkaConsumerConfiguration
{
    public const string ConfigurationSection = "KafkaConfiguration:Consumer";

    public AutoOffsetReset AutoOffsetReset { get; set; }
    public string Topic { get; set; }
    
    public string GroupId { get; set; }
}