using DryIoc;
using Microsoft.Extensions.Configuration;
using StriBot.DryIoc.Registrators;

namespace StriBot.DryIoc;

public static class GlobalContainer
{
    public static IContainer Default { get; private set; }

    public static void Initialize(IConfiguration configuration)
    {
        Default = InitializeContainer(configuration);
    }

    private static IContainer InitializeContainer(IConfiguration configuration)
        => new Container()
            .RegisterConfiguration(configuration)
            .RegisterFillers();
}