using RocketGateway.Exceptions;

namespace RocketGateway.Extensions;

public static class ConfigurationExtensions
{
    public static TModel GetModelOrThrow<TModel>(
        this IConfiguration configuration,
        string configurationSection
        )
    {
        TModel resultConfiguration = configuration.GetSection(configurationSection)
            .Get<TModel>() ?? throw new ConfigurationException(
            $"Failed to bind configuration on {configurationSection}"
        );
        return resultConfiguration;
    }
}