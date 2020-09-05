using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace StriBot
{
    public static class Reporter
    {
        private static string _catalog = "Отчеты";
        private static string _infoName = "Информационные";
        private static string _interactiveName = "Интерактив";
        private static string _ordersName = "Заказы";
        private static string _moderatorsName = "Модераторам";
        private static string _streamersName = "Стримеры";

        public static void CreateReport()
        {
            string name = DateTime.Now.ToString(new CultureInfo("ru-RU")).Split(' ')[0];

            string path = GetPath(name);

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
            string path = GetPath(fileName);

            List<string> report = new List<string>();

            foreach (var command in commands.Values)
            {
                if (command.Type == commandType)
                {
                    StringBuilder result = new StringBuilder('!' + command.Name);
                    if (command.Args != null)
                        foreach (var arg in command.Args)
                            result.Append(String.Format(" [{0}]", arg));
                    result.Append(String.Format(" - {0}", command.Info));
                    if (command.Requires == Role.Moderator)
                        result.Append(". Только для модератора");
                    report.Add(result.ToString());
                }
            }

            File.WriteAllLines(path, report.ToArray());
        }

        private static void CommandReport(string fileName, Dictionary<string, Command> commands, Role role)
        {
            string path = GetPath(fileName);

            List<string> report = new List<string>();

            foreach (var command in commands.Values)
            {
                if (command.Requires == role)
                {
                    StringBuilder result = new StringBuilder('!' + command.Name);
                    if (command.Args != null)
                        foreach (var arg in command.Args)
                            result.Append(String.Format(" [{0}]", arg));
                    result.Append(String.Format(" - {0}", command.Info));
                    report.Add(result.ToString());
                }
            }

            File.WriteAllLines(path, report.ToArray());
        }

        private static string GetPath(string name)
            => string.Format("{0}\\{1}.txt", _catalog, name);

        private static void CreateCatalogIfNeed()
        {
            if (!Directory.Exists(_catalog))
                Directory.CreateDirectory(_catalog);
        }
    }
}
