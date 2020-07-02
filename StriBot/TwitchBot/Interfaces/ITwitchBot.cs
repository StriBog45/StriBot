using System;
using System.Collections.Generic;

namespace StriBot.TwitchBot.Interfaces
{
    public interface ITwitchBot
    {
        void SendMessage(string message);

        void CreateCommands();

        void TimerTick();

        void SetConstructorSettings(Action<List<(string, string, int)>> ordersUpdate, Action bossUpdate, Action deathUpdate);

        void CreateBets(string[] options);

        void StopBetsProcess();

        void SetBetsWinner(int winner);

        void DistributionMoney(int perUser, int maxUsers, bool bonus = true);

        string TextReminder { get; set; }

        void Reconnect();

        void SmileMode();

        void SubMode();

        void FollowersMode();

        void FollowersModeOff();

        int Deaths { get; set; }

        CollectionHelper Bosses { get; set; }

        List<(string, string, int)> ListOrders { get; set; }

        Dictionary<string, Command> Commands { get; set; }
    }
}