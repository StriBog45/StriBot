using System;
using System.Runtime.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StriBot.DependencyInjections;

namespace StriBot;

internal static class Program
{
    private const string ConfigFileName = "appsettings.json";

    /// <summary>
    /// Главная точка входа для приложения.
    /// </summary>
    [STAThread]
    [SupportedOSPlatform("windows")]
    private static void Main()
    {
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

        var hostApplicationBuilder = Host.CreateApplicationBuilder();
        hostApplicationBuilder.Configuration.AddJsonFile(ConfigFileName);

        hostApplicationBuilder.Services.AddSingleton<Form1>()
            .AddSpeaker()
            .AddBot()
            .AddBotHandlers()
            .AddBotCommands()
            .AddLanguage()
            .AddDataBase();

        using var host = hostApplicationBuilder.Build();

        System.Windows.Forms.Application.Run(host.Services.GetRequiredService<Form1>());

        try
        {
            host.Run();
        }
        catch (OperationCanceledException)
        {
        }
    }
}