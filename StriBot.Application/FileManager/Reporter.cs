using System.Collections.Generic;
using System.IO;
using System.Text;
using StriBot.Application.Bot.Enums;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Commands.Models;

namespace StriBot.Application.FileManager;

public static class Reporter
{
    private const string Catalog = "Отчеты";
    private const string InfoName = "Информационные";
    private const string InteractiveName = "Интерактив";
    private const string OrdersName = "Заказы";
    private const string ModeratorsName = "Модераторам";
    private const string StreamersName = "Стримеры";

    public static void CreateCommands(Dictionary<string, Command> commands)
    {
        CreateCatalogIfNeed();

        CommandReport(InfoName, commands, CommandType.Info);
        CommandReport(InteractiveName, commands, CommandType.Interactive);
        CommandReport(OrdersName, commands, CommandType.Order);
        CommandReport(ModeratorsName, commands, Role.Moderator);
        CommandReport(StreamersName, commands, CommandType.Streamers);
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
        => $"{Catalog}\\{name}.txt";

    private static void CreateCatalogIfNeed()
    {
        if (!Directory.Exists(Catalog))
            Directory.CreateDirectory(Catalog);
    }
}