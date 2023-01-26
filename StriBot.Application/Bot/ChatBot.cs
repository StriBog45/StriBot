using System.Collections.Generic;
using System.Threading.Tasks;
using StriBot.Application.Bot.Enums;
using StriBot.Application.Bot.Handlers;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Commands.Extensions;
using StriBot.Application.Commands.Handlers;
using StriBot.Application.Commands.Handlers.Progress;
using StriBot.Application.Commands.Handlers.Raffle;
using StriBot.Application.Commands.Models;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.Events;
using StriBot.Application.Events.Enums;
using StriBot.Application.Events.Models;
using StriBot.Application.Localization.Extensions;
using StriBot.Application.Localization.Implementations;
using StriBot.Application.Platforms.Enums;
using StriBot.Application.Speaker.Interfaces;
using StriBot.Bots;

namespace StriBot.Application.Bot
{
    public class ChatBot
    {
        public Dictionary<string, Command> Commands { get; }

        private int _timer;
        private readonly CurrencyBaseHandler _currencyBaseHandler;
        private readonly HalberdHandler _halberdHandler;
        private readonly DuelHandler _duelHandler;
        private readonly ISpeaker _speaker;
        private readonly BetsHandler _betsHandler;
        private readonly TwitchBot _twitchBot;
        private readonly RememberHandler _rememberHandler;
        private readonly Currency _currency;
        private readonly RewardHandler _rewardHandler;
        private readonly IDataBase _dataBase;

        public ChatBot(ISpeaker speaker, 
            TwitchBot twitchBot, 
            DuelHandler duelHandler, 
            HalberdHandler halberdHandler,
            CurrencyBaseHandler currencyBaseHandler,
            BetsHandler betsHandler, 
            RememberHandler rememberHandler,
            Currency currency, 
            RaffleHandler raffleHandler, 
            RewardHandler rewardHandler, 
            IDataBase dataBase,
            MMRHandler mmrHandler,
            LinkHandler linkHandler,
            BurgerHandler burgerHandler,
            RandomAnswerHandler randomAnswerHandler,
            OrderHandler orderHandler,
            ProgressHandler progressHandler)
        {
            _speaker = speaker;
            _twitchBot = twitchBot;
            _halberdHandler = halberdHandler;
            _currencyBaseHandler = currencyBaseHandler;
            _duelHandler = duelHandler;
            _betsHandler = betsHandler;
            _rememberHandler = rememberHandler;
            _currency = currency;
            _rewardHandler = rewardHandler;
            _dataBase = dataBase;

            EventContainer.CommandReceived += OnChatCommandReceived;
            EventContainer.PlatformEventReceived += OnPlatformEventReceived;
            EventContainer.RewardEventReceived += OnRewardEventReceived;

            Commands = new Dictionary<string, Command>
            {
                linkHandler.CreateCommands(),
                mmrHandler.CreateCommands(),
                randomAnswerHandler.CreateCommands(),
                burgerHandler.CreateCommands(),
                progressHandler.CreateCommands(),
                _rememberHandler.CreateCommands(),
                orderHandler.CreateCommands(),
                _currencyBaseHandler.CreateCommands(),
                _duelHandler.CreateCommands(),
                _halberdHandler.CreateCommands(),
                _betsHandler.CreateCommands(),
                raffleHandler.CreateCommands()
            };
        }

        private async Task OnRewardEventReceived(RewardInfo rewardInfo)
            => await _rewardHandler.Handle(rewardInfo);

        private void OnPlatformEventReceived(PlatformEventInfo platformEventInfo)
        {
            switch (platformEventInfo.EventType)
            {
                case PlatformEventType.HighlightedMessage:
                    HighlightedMessage(platformEventInfo);
                    break;
                case PlatformEventType.Raid:
                    Raid(platformEventInfo);
                    break;
                case PlatformEventType.GiftSubscription:
                    GiftSubscription(platformEventInfo);
                    break;
                case PlatformEventType.NewSubscription:
                case PlatformEventType.ReSubscription:
                    Subsctiption(platformEventInfo);
                    break;
                case PlatformEventType.Message:
                    _currencyBaseHandler.ReceivedMessage(platformEventInfo.UserName);
                    break;
            }
        }

        private void Subsctiption(PlatformEventInfo platformEventInfo)
        {
            EventContainer.Message($"{platformEventInfo.UserName} подписался! PogChamp Срочно плед этому господину! А пока возьми {PriceList.ToysForSub} {_currency.Incline(PriceList.ToysForSub, true)} :)", platformEventInfo.Platform);
            _dataBase.AddMoney(platformEventInfo.UserName, PriceList.ToysForSub);
            _speaker.Say("Спасибо за подписку!");
        }

        private void GiftSubscription(PlatformEventInfo platformEventInfo)
        {
            EventContainer.Message($"{platformEventInfo.UserName} подарил подписку для {platformEventInfo.SecondName}! PogChamp Спасибо большое! Прими нашу небольшую благодарность в качестве {_currency.Incline(PriceList.ToysForSub)}", platformEventInfo.Platform);
            _dataBase.AddMoney(platformEventInfo.UserName, PriceList.ToysForSub);
            _speaker.Say("Спасибо за подарочную подписку!");
        }

        private void HighlightedMessage(PlatformEventInfo platformEventInfo)
            => _speaker.Say(platformEventInfo.Message);

        private void Raid(PlatformEventInfo platformEventInfo)
        {
            EventContainer.Message($"Нас атакует армия {platformEventInfo.UserName}! Поднимаем щиты! PurpleStar PurpleStar PurpleStar ", platformEventInfo.Platform);
            _speaker.Say("Нас атакуют! Поднимайте щиты!");
        }

        public void TimerTick()
        {
            _timer++;

            _betsHandler.Tick(new Platform[] { Platform.Twitch });

            // if (_timer == 40)
            //     _currencyBaseManager.DistributionMoney(1, 5, Platform.Twitch);
            if (_timer == 15)
                SendMessage("Если увидел крутой момент, запечатли это! Сделай клип! striboF ");
            if (_timer == 30)
                SendMessage("Поддержи нас на второй площадке! <3 https://vkplay.live/stribog45");
            if (_timer == 45)
                SendMessage("Спасибо за вашу поддержку! <3 ");
            if (_timer == 60)
                SendMessage("Список команд https://vk.cc/a6Giqf");

            _rememberHandler.Tick(_timer);
            _duelHandler.Tick();
            _halberdHandler.Tick();

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
                _speaker.Say("Бот подключился");
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

            if (_twitchBot.IsConnected())
                _speaker.Say("Бот переподключился");
        }

        private void OnChatCommandReceived(CommandInfo commandInfo)
        {
            if (Commands.ContainsKey(commandInfo.CommandText))
            {
                if (IsAccessAllowed(Commands[commandInfo.CommandText].Requires, commandInfo)
                    && _halberdHandler.CanSendMessage(commandInfo))
                    Commands[commandInfo.CommandText].Action(commandInfo);
            }
            else if (Commands.ContainsKey(commandInfo.CommandText.ToLower()))
            {
                if (IsAccessAllowed(Commands[commandInfo.CommandText.ToLower()].Requires, commandInfo)
                    && _halberdHandler.CanSendMessage(commandInfo))
                    Commands[commandInfo.CommandText.ToLower()].Action(commandInfo);
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
            => EventContainer.Message(text, Platform.Twitch);
    }
}