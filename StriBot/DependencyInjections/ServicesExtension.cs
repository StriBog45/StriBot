using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using StriBot.Application.Bot;
using StriBot.Application.Bot.Handlers;
using StriBot.Application.Commands.Handlers;
using StriBot.Application.Commands.Handlers.Progress;
using StriBot.Application.Commands.Handlers.Raffle;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.FileManager;
using StriBot.Application.Localization;
using StriBot.Application.Localization.Implementations;
using StriBot.Application.Speaker.Interfaces;
using StriBot.Application.Twitch;
using StriBot.Application.Twitch.Interfaces;
using StriBot.DateBase.Implementations;
using StriBot.Speakers;

namespace StriBot.DependencyInjections;

public static class ServicesExtension
{
    [SupportedOSPlatform("windows")]
    public static IServiceCollection AddSpeaker(this IServiceCollection services)
    {
        return services.AddSingleton<ISpeaker, Speaker>();
    }

    public static IServiceCollection AddBot(this IServiceCollection services)
    {
        return services.AddSingleton<ITwitchInfo, TwitchInfo>()
            .AddSingleton<SettingsFileManager>()
            .AddSingleton<ChatBot>()
            .AddSingleton<TwitchBot>()
            .AddTransient<TwitchApiClient>()
            .AddTransient<TwitchAuthorization>()
            .AddSingleton<RepeatMessagesHandler>();
    }

    public static IServiceCollection AddLanguage(this IServiceCollection services)
    {
        return services.AddSingleton<Currency>()
            .AddTransient<Minute>()
            .AddTransient<ReadyMadePhrases>();
    }

    public static IServiceCollection AddBotHandlers(this IServiceCollection services)
    {
        return services.AddSingleton<RewardHandler>();
    }

    public static IServiceCollection AddBotCommands(this IServiceCollection services)
    {
        return services.AddSingleton<MMRHandler>()
            .AddSingleton<OrderHandler>()
            .AddSingleton<CurrencyBaseHandler>()
            .AddSingleton<HalberdHandler>()
            .AddSingleton<DuelHandler>()
            .AddTransient<CustomCommandHandler>()
            .AddSingleton<RandomAnswerHandler>()
            .AddSingleton<BurgerHandler>()
            .AddSingleton<BetsHandler>()
            .AddSingleton<ProgressHandler>()
            .AddSingleton<RememberHandler>()
            .AddSingleton<RaffleHandler>()
            .AddSingleton<AnswerOptions>()
            .AddTransient<BananaHandler>();
    }

    public static IServiceCollection AddDataBase(this IServiceCollection services)
    {
        return services.AddTransient<IDataBase, DataBase>();
    }
}