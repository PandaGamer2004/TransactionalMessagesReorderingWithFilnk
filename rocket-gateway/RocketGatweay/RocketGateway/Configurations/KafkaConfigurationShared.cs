namespace RocketGateway.Configurations;

public class KafkaConfigurationShared
{
    public const string ConfigurationKey = "KafkaConfiguration:Shared";

    public string BootstrapServer { get; set; } = string.Empty;
}