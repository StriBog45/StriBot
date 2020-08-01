using DryIoc;
using StriBot.Bots.Enums;
using StriBot.Commands;
using StriBot.Commands.CommonFunctions;
using StriBot.CustomData;
using StriBot.DryIoc;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using StriBot.Speakers;
using System;
using System.Collections.Generic;

namespace StriBot.Bots
{
    public class ChatBot
    {
        public Dictionary<string, Command> Commands { get; set; }
        
        public string TextReminder { get; set; } = string.Empty;
        
        private AnswerOptions _customArray;
        private int _timer;
        private int toysForSub = 30;
        private readonly CurrencyBaseManager _currencyBaseManager;
        private readonly HalberdManager _halberdManager;
        private readonly DuelManager _duelManager;
        private readonly Speaker _speaker;
        private readonly BetsManager _betsManager;
        private readonly TwitchBot _twitchBot;

        public ChatBot(Speaker speaker, AnswerOptions customArray, TwitchBot twitchBot, DuelManager duelManager, HalberdManager halberdManager, CurrencyBaseManager currencyBaseManager, BetsManager betsManager)
        {
            _customArray = customArray;
            _speaker = speaker;
            _twitchBot = twitchBot;
            _halberdManager = halberdManager;
            _currencyBaseManager = currencyBaseManager;
            _duelManager = duelManager;
            _betsManager = betsManager;

            GlobalEventContainer.CommandReceived += OnChatCommandReceived;
        }

        public void TimerTick()
        {
            _timer++;

            _betsManager.Tick(new Platform[] { Platform.Twitch });

            if (_timer == 40)
                _currencyBaseManager.DistributionMoney(1, 5, Enums.Platform.Twitch);
            if (_timer == 15)
                SendMessage("Если увидел крутой момент, запечатли это! Сделай клип! striboF ");
            if (_timer == 30)
                SendMessage("У стримера все под контролем! striboPled ");
            if (_timer == 45)
                SendMessage("Спасибо за вашу поддержку! HolidaySanta ");

            if (_timer % 10 == 0 && !string.IsNullOrEmpty(TextReminder))
                SendMessage("Напоминание: " + TextReminder);

            if (_timer == 60)
                _timer = 0;

            _duelManager.Tick();
            _halberdManager.Tick();
        }

        public void Connect(Platform[] platforms)
        {
            foreach (var platform in platforms)
            {
                switch (platform)
                {
                    case Platform.Twitch:
                        _twitchBot.Connect();
                        break;
                }
            }
        }

        internal void Reconnect(Platform[] platforms)
        {
            foreach (var platform in platforms)
            {
                switch (platform)
                {
                    case Platform.Twitch:
                        _twitchBot.Reconnect();
                        break;
                }
            }
        }

        public void CreateCommands()
        {
            var container = GlobalContainer.Default;
            var managerMMR = container.Resolve<MMRManager>();
            var readyMadePhrases = container.Resolve<ReadyMadePhrases>();
            var orderManager = container.Resolve<OrderManager>();
            var linkManager = container.Resolve<LinkManager>();
            var randomAnswerManager = container.Resolve<RandomAnswerManager>();
            var burgerManager = container.Resolve<BurgerManager>();
            var progressManager = container.Resolve<ProgressManager>();

            Commands = new Dictionary<string, Command>()
            {
                linkManager.CreateCommands(),
                managerMMR.CreateCommands(),
                randomAnswerManager.CreateCommands(),
                burgerManager.CreateCommands(),
                progressManager.CreateCommands(),
                //{ "uptime", new Command("Uptime","Длительность текущей трансляции",
                //delegate (CommandInfo e) {
                //    SendMessage( $"Трансляция длится: { GetUptime()}"); }, CommandType.Info)},

                { "напомнить", new Command("Напомнить", "Создает напоминалку. При использовании без указания текста, напоминание будет удалено", Role.Moderator,
                delegate (CommandInfo e) {
                    TextReminder = e.ArgumentsAsString;
                    if(TextReminder.Length > 0)
                        SendMessage($"Напоминание: \"{e.ArgumentsAsString}\" создано");
                    else
                        SendMessage("Напоминание удалено");
                }, new string[] {"текст"}, CommandType.Interactive )},

                orderManager.CreateCommands(),
                _currencyBaseManager.CreateCommands(),
                _duelManager.CreateCommands(),
                _halberdManager.CreateCommands(),
                _betsManager.CreateCommands()
            };
        }

        private void OnChatCommandReceived(CommandInfo commandInfo)
        {
            if (Commands.ContainsKey(commandInfo.CommandText))
            {
                if (IsAccessAllowed(Commands[commandInfo.CommandText].Requires, commandInfo)
                    && _halberdManager.CanSendMessage(commandInfo))
                    Commands[commandInfo.CommandText].Action(commandInfo);
            }
        }

        private bool IsAccessAllowed(Role role, CommandInfo commandInfo)
        {
            var result = true;

            switch (role)
            {
                case Role.Subscriber:
                    result = commandInfo.IsSubscriber.HasValue && commandInfo.IsSubscriber.Value;
                    break;
                case Role.Moderator:
                    result = (commandInfo.IsModerator.HasValue && commandInfo.IsModerator.Value) || (commandInfo.IsBroadcaster.HasValue && commandInfo.IsBroadcaster.Value);
                    break;
                case Role.Broadcaster:
                    result = commandInfo.IsBroadcaster.HasValue && commandInfo.IsBroadcaster.Value; 
                    break;
            }

            return result;
        }

        private void SendMessage(string text)
            => GlobalEventContainer.Message(text, Platform.Twitch);
    }
}