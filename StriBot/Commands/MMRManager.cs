using StriBot.Bots.Enums;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using System.Collections.Generic;

namespace StriBot.Commands
{
    public class MMRManager
    {
        public int MMR { get; set; } = 4400;
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        private int MMRChange = 30;
        private string medallion = "Властелин 3";

        public MMRManager()
        {
        }

        public Command AddWin()
        {
            var result = new Command("Победа", "Добавляет победу", Role.Moderator,
                delegate (CommandInfo commandInfo)
                {
                    Wins++;
                    MMR += MMRChange;
                    GlobalEventContainer.Message($"Побед: {Wins}, Поражений: {Losses}", commandInfo.Platform);
                }, CommandType.Interactive);

            return result;
        }

        private void SendCurrentAccount(Platform platform)
            => GlobalEventContainer.Message($"Побед: {Wins}, Поражений: {Losses}", platform);

        public Command AddLose()
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

        public Command CurrentAccount()
            => new Command("Счет", "Текущий счет побед и поражений",
                delegate (CommandInfo commandInfo)
                {
                    SendCurrentAccount(commandInfo.Platform);
                }, CommandType.Info);

        public Command CurrentMMR()
            => new Command("mmr", "Узнать рейтинг стримера в Dota 2",
                delegate (CommandInfo commandInfo)
                {
                    GlobalEventContainer.Message($"Рейтинг: {MMR} Звание: {medallion}", commandInfo.Platform);
                }, CommandType.Info);

        public Command CheckMMR()
            => new Command("CheckMMR", "Узнать рейтинг объекта",
                delegate (CommandInfo commandInfo)
                {
                    if (string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                        GlobalEventContainer.Message($"Ваш рейтинг: {RandomHelper.random.Next(1, 7000)}", commandInfo.Platform);
                    else
                        GlobalEventContainer.Message($"Рейтинг {commandInfo.ArgumentsAsString}: {RandomHelper.random.Next(1, 10000)}", commandInfo.Platform);
                }, new string[] { "Объект" }, CommandType.Interactive);

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>()
            {
                CurrentMMR(),
                CurrentAccount(),
                AddWin(),
                AddLose(),
                CheckMMR()
            };
    }
}