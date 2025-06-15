using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StriBot.Application.Loader;
using StriBot.Application.WebSockets;
using StriBot.DependencyInjections;
using TwitchLib.EventSub.Websockets.Extensions;
using WindowsFormsLifetime;

namespace StriBot;

internal static class Program
{
    private const string ConfigFileName = "appsettings.json";

    /// <summary>
    /// Главная точка входа для приложения.
    /// </summary>
    [STAThread]
    [SupportedOSPlatform("windows")]
    private static async Task Main()
    {
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

        var hostApplicationBuilder = Host.CreateApplicationBuilder();
        hostApplicationBuilder.Configuration.AddJsonFile(ConfigFileName);

        hostApplicationBuilder.Services.AddSingleton<Form1>()
            .AddSpeaker()
            .AddLoader()
            .AddBot()
            .AddBotHandlers()
            .AddBotCommands()
            .AddLanguage()
            .AddDataBase()
            .AddLogging()
            .AddTwitchLibEventSubWebsockets()
            .AddHostedService<WebsocketHostedService>();

        hostApplicationBuilder.UseWindowsFormsLifetime<Form1>();

        using var host = hostApplicationBuilder.Build();

        await host.Services.GetService<Loader>().Load();

        await host.RunAsync();
    }
}