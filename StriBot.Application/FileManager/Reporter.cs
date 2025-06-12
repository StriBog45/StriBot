using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using StriBot.Application.Bot.Enums;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Commands.Models;

namespace StriBot.Application.FileManager;

public static class Reporter
{
    private static readonly string _catalog = "Отчеты";
    private static readonly string _infoName = "Информационные";
    private static readonly string _interactiveName = "Интерактив";
    private static readonly string _ordersName = "Заказы";
    private static readonly string _moderatorsName = "Модераторам";
    private static readonly string _streamersName = "Стримеры";

    public static void CreateReport()
    {
        var name = DateTime.Now.ToString(new CultureInfo("ru-RU")).Split(' ')[0];
        var path = GetPath(name);

        while (File.Exists(path))
        {
            name += "(1)";
            path = GetPath(name);
        }
    }

    public static void CreateCommands(Dictionary<string, Command> commands)
    {
        CreateCatalogIfNeed();

        CommandReport(_infoName, commands, CommandType.Info);
        CommandReport(_interactiveName, commands, CommandType.Interactive);
        CommandReport(_ordersName, commands, CommandType.Order);
        CommandReport(_moderatorsName, commands, Role.Moderator);
        CommandReport(_streamersName, commands, CommandType.Streamers);
    }

    private static void CommandReport(string fileName, Dictionary<string, Command> commands, CommandType commandType)
    {
        var path = GetPath(fileName);
        var report = new List<string>();

        foreach (var command in commands.Values)
        {
            if (command.Type == commandType)
            {
                var result = new StringBuilder('!' + command.Name);
                if (command.Args != null)
                    foreach (var arg in command.Args)
                        result.Append($" [{arg}]");
                result.Append($" - {command.Info}");
                if (command.Requires == Role.Moderator)
                    result.Append(". Только для модератора");
                report.Add(result.ToString());
            }
        }

        File.WriteAllLines(path, report.ToArray());
    }

    private static void CommandReport(string fileName, Dictionary<string, Command> commands, Role role)
    {
        var path = GetPath(fileName);
        var report = new List<string>();

        foreach (var command in commands.Values)
        {
            if (command.Requires == role)
            {
                var result = new StringBuilder('!' + command.Name);
                if (command.Args != null)
                    foreach (var arg in command.Args)
                        result.Append($" [{arg}]");
                result.Append($" - {command.Info}");
                report.Add(result.ToString());
            }
        }

        File.WriteAllLines(path, report.ToArray());
    }

    private static string GetPath(string name)
        => $"{_catalog}\\{name}.txt";

    private static void CreateCatalogIfNeed()
    {
        if (!Directory.Exists(_catalog))
            Directory.CreateDirectory(_catalog);
    }
}