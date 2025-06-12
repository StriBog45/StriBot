using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StriBot.Application.Bot;
using StriBot.Application.Bot.Handlers;
using StriBot.Application.Commands.Handlers;
using StriBot.Application.Commands.Handlers.Progress;
using StriBot.Application.Commands.Handlers.Raffle;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.FileManager;
using StriBot.Application.Localization;
using StriBot.Application.Localization.Implementations;
using StriBot.Application.Platforms.Enums;
using StriBot.Application.Speaker.Interfaces;
using StriBot.Application.Twitch;
using StriBot.Application.Twitch.Interfaces;
using StriBot.ConsoleView.Speakers.Implementations;

namespace StriBot.ConsoleView;

internal class Program
{
    static void Main()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            // BotFiller
            .AddSingleton<ITwitchInfo, TwitchInfo>()
            .AddSingleton<SettingsFileManager>()
            .AddSingleton<ChatBot>()
            .AddSingleton<TwitchBot>()
            .AddTransient<TwitchApiClient>()

            // BotHandlersFiller
            .AddSingleton<RewardHandler>()

            // CommandCommonFunctionsFiller
            .AddTransient<ReadyMadePhrases>()

            // CommandsFiller
            .AddSingleton<MMRHandler>()
            .AddSingleton<OrderHandler>()
            .AddSingleton<CurrencyBaseHandler>()
            .AddSingleton<HalberdHandler>()
            .AddSingleton<DuelHandler>()
            .AddSingleton<CustomCommandHandler>()
            .AddSingleton<RandomAnswerHandler>()
            .AddSingleton<BurgerHandler>()
            .AddSingleton<BetsHandler>()
            .AddSingleton<ProgressHandler>()
            .AddSingleton<RememberHandler>()
            .AddSingleton<RaffleHandler>()
            .AddSingleton<AnswerOptions>()

            // DataBase
            .AddSingleton<IDataBase, DataBase.Implementations.DataBase>()

            // LanguageFiller
            .AddSingleton<Currency>()
            .AddSingleton<Minute>()

            // Speaker Filler
            .AddSingleton<ISpeaker, SpeakerEmpty>()
            .BuildServiceProvider();

        var logger = serviceProvider.GetService<ILoggerFactory>()
            .CreateLogger<Program>();
        logger.LogDebug("Starting application");

        //do the actual work here
        var chatBot = serviceProvider.GetService<ChatBot>();
        chatBot.Connect(new [] {Platform.Twitch});

        logger.LogDebug("Application started!");

        StartConsoleMenu();
    }

    private static void StartConsoleMenu()
    {
        Console.WriteLine("StriBot started!");
        Console.WriteLine();
        WriteHelpMenu();

        while (true)
        {
            var command = Console.ReadLine()?.ToLower();
            switch (command)
            {
                case "help":
                    WriteHelpMenu();
                    break;
                case "exit":
                    return;
            }
        }

        static void WriteHelpMenu()
        {
            Console.WriteLine("Help - Write console commands");
            Console.WriteLine("Exit - Stop application");
        }
    }
}