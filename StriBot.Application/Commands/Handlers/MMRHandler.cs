using System.Collections.Generic;
using StriBot.Application.Bot.Enums;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Commands.Extensions;
using StriBot.Application.Commands.Models;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;
using StriBot.Application.Extensions;
using StriBot.Application.Platforms.Enums;

namespace StriBot.Application.Commands.Handlers
{
    public class MMRHandler
    {
        public int MMR { get; set; } = 4400;
        public int Wins { get; set; }
        public int Losses { get; set; }

        private const int MMRChange = 30;
        private const string Medallion = "Властелин 3";

        private Command AddWin()
        {
            var result = new Command("Победа", "Добавляет победу", Role.Moderator,
                delegate (CommandInfo commandInfo)
                {
                    Wins++;
                    MMR += MMRChange;
                    EventContainer.Message($"Побед: {Wins}, Поражений: {Losses}", commandInfo.Platform);
                }, CommandType.Interactive);

            return result;
        }

        private void SendCurrentAccount(Platform platform)
            => EventContainer.Message($"Побед: {Wins}, Поражений: {Losses}", platform);

        private Command AddLose()
        {
            var result = new Command("Поражение", "Добавляет поражение", Role.Moderator,
                delegate (CommandInfo commandInfo)
                {
                    Losses++;
                    MMR -= MMRChange;
                    SendCurrentAccount(commandInfo.Platform);
                }, CommandType.Interactive);

            return result;
        }

        private Command CurrentAccount()
            => new Command("Счет", "Текущий счет побед и поражений",
                delegate (CommandInfo commandInfo)
                {
                    SendCurrentAccount(commandInfo.Platform);
                }, CommandType.Info);

        private Command CurrentMMR()
            => new Command("mmr", "Узнать рейтинг стримера в Dota 2",
                delegate (CommandInfo commandInfo)
                {
                    EventContainer.Message($"Рейтинг: {MMR} Звание: {Medallion}", commandInfo.Platform);
                }, CommandType.Info);

        private Command CheckMMR()
            => new Command("CheckMMR", "Узнать рейтинг объекта",
                delegate (CommandInfo commandInfo)
                {
                    if (string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                        EventContainer.Message($"Ваш рейтинг: {RandomHelper.Random.Next(1, 7000)}", commandInfo.Platform);
                    else
                        EventContainer.Message($"Рейтинг {commandInfo.ArgumentsAsString}: {RandomHelper.Random.Next(1, 10000)}", commandInfo.Platform);
                }, new[] { "Объект" }, CommandType.Interactive);

        public Dictionary<string, Command> CreateCommands()
            => new()
            {
                CurrentMMR(),
                CurrentAccount(),
                AddWin(),
                AddLose(),
                CheckMMR()
            };
    }
}