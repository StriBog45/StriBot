using DryIoc;
using Microsoft.Extensions.Configuration;

namespace StriBot.DryIoc;

public static class ConfigurationFiller
{
    public static IContainer RegisterConfiguration(this IContainer container, IConfiguration configuration)
    {
        container.Use(configuration);
        return container;
    }
}