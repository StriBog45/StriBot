using System;
using System.Collections.Generic;

namespace StriBot.TwitchBot.Interfaces
{
    public interface ITwitchBot
    {
        void SendMessage(string message);

        void UserTimeout(string userName, TimeSpan timeSpan, string timeoutText);

        void CreateCommands();

        void TimerTick();

        void SetConstructorSettings(Action bossUpdate, Action deathUpdate);

        void CreateBets(string[] options);

        void StopBetsProcess();

        void SetBetsWinner(int winner);

        string TextReminder { get; set; }

        void Reconnect();

        void SmileMode();

        void SubMode();

        void FollowersMode();

        void FollowersModeOff();

        int Deaths { get; set; }

        CollectionHelper Bosses { get; set; }

        Dictionary<string, Command> Commands { get; set; }
    }
}