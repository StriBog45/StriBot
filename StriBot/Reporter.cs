using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace StriBot
{
    public static class Reporter
    {
        private static string Catalog { get; set; } = "Отчеты";
        private static string InfoName { get; } = "Информационные";
        private static string InteractiveName { get; } = "Интерактив";
        private static string OrdersName { get; } = "Заказы";
        private static string ModeratorsName { get; } = "Модераторам";
        private static string StreamersName { get; } = "Стримеры";

        public static void CreateReport()
        {
            string name = String.Format("{0}", DateTime.Now.ToString(new CultureInfo("ru-RU")).Split(' ')[0]);

            string path = GetPath(name);

            while (File.Exists(path))
            {
                name += "(1)";
                path = GetPath(name);
            }

            //List<string> report = new List<string>();
            //report.Add(String.Format("Побед: {0}, Поражений: {1}", MyBot.Wins, MyBot.Losses));
            //report.Add(String.Format("Смертей: {0}", MyBot.Deaths));
            //report.Add(String.Format("Боссы: {0}", MyBot.bosses.ToString()));

        }

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
            => String.Format("{0}\\{1}.txt", Catalog, name);

        private static void CreateCatalogIfNeed()
        {
            if (!Directory.Exists(Catalog))
                Directory.CreateDirectory(Catalog);
        }
    }
}
