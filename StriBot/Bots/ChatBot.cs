using DryIoc;
using StriBot.Bots.Enums;
using StriBot.Commands;
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
        private int _timer;
        private readonly CurrencyBaseManager _currencyBaseManager;
        private readonly HalberdManager _halberdManager;
        private readonly DuelManager _duelManager;
        private readonly Speaker _speaker;
        private readonly BetsManager _betsManager;
        private readonly TwitchBot _twitchBot;
        private readonly RememberManager _rememberManager;

        public ChatBot(Speaker speaker, TwitchBot twitchBot, DuelManager duelManager, HalberdManager halberdManager, CurrencyBaseManager currencyBaseManager, BetsManager betsManager, RememberManager rememberManager)
        {
            _speaker = speaker;
            _twitchBot = twitchBot;
            _halberdManager = halberdManager;
            _currencyBaseManager = currencyBaseManager;
            _duelManager = duelManager;
            _betsManager = betsManager;
            _rememberManager = rememberManager;

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

            _rememberManager.Tick(_timer);
            _duelManager.Tick();
            _halberdManager.Tick();

            if (_timer == 60)
                _timer = 0;
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

            if (_twitchBot.IsConnected())
                _speaker.Say("Бот подключился к Twitch");
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

                _rememberManager.CreateCommands(),
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