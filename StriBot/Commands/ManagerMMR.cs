using StriBot.TwitchBot.Interfaces;
using System;
using TwitchLib.Client.Events;

namespace StriBot.Commands
{
    public class ManagerMMR
    {
        public int MMR { get; set; } = 4400;
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;

        private readonly ITwitchBot twitchBot;
        private int MMRChange = 30;
        private string medallion = "Властелин 3";

        public ManagerMMR(ITwitchBot twitchBot)
        {
            this.twitchBot = twitchBot;
        }

        public Command AddWin()
        {
            var result = new Command("Победа", "Добавляет победу", Role.Moderator,
                delegate (OnChatCommandReceivedArgs e)
                {
                    Wins++;
                    MMR += MMRChange;
                    twitchBot.SendMessage($"Побед: {Wins}, Поражений: {Losses}");
                }, CommandType.Interactive);

            return result;
        }

        private void SendCurrentAccount()
            => twitchBot.SendMessage($"Побед: {Wins}, Поражений: {Losses}");

        public Command AddLose()
        {
            var result = new Command("Поражение", "Добавляет поражение", Role.Moderator,
                delegate (OnChatCommandReceivedArgs e)
                {
                    Losses++;
                    MMR -= MMRChange;
                    SendCurrentAccount();
                }, CommandType.Interactive);

            return result;
        }

        public Command CurrentAccount()
            => new Command("Счет", "Текущий счет побед и поражений",
                delegate (OnChatCommandReceivedArgs e)
                {
                    SendCurrentAccount();
                }, CommandType.Info);

        public Command CurrentMMR()
            => new Command("mmr", "Узнать рейтинг стримера в Dota 2",
                delegate (OnChatCommandReceivedArgs e)
                {
                    twitchBot.SendMessage($"Рейтинг: {MMR} Звание: {medallion}");
                }, CommandType.Info);
    }
}