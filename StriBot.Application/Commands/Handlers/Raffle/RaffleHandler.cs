using System.Collections.Generic;
using StriBot.Application.Bot.Enums;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Commands.Extensions;
using StriBot.Application.Commands.Handlers.Raffle.Models;
using StriBot.Application.Commands.Models;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;
using StriBot.Application.Extensions;
using StriBot.Application.Platforms.Enums;

namespace StriBot.Application.Commands.Handlers.Raffle
{
    public class RaffleHandler
    {
        private readonly IDataBase _dataBase;
        private readonly List<RaffleParticipant> _participantsList;
        private string _commandName;
        private int _price;
        
        private const int SubscriberBonus = 3;
        private int _participantsCount;

        private bool IsProgress { get; set; }

        public RaffleHandler(IDataBase dataBase)
        {
            _dataBase = dataBase;
            _participantsList = new List<RaffleParticipant>();
        }

        public string GetCurrentCommandName()
            => _commandName;

        public void RaffleStart(string name, int price)
        {
            _participantsList.Clear();
            IsProgress = true;
            _commandName = name;
            _participantsCount = 0;
            _price = price;
            EventContainer.Message($"Розыгрыш начался! Пиши команду !{_commandName} И (ссылка на торговлю в steam)", Platform.Twitch);
        }

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>()
            {
                RaffleInfo()
            };

        private Command RaffleInfo()
        {
            var result = new Command("розыгрыш", "информация по розыгрышу", Role.Any,
                delegate (CommandInfo commandInfo)
                {
                    var commandText = !IsProgress
                        ? ", которую укажет стример"
                        : $" !{_commandName}";
                    EventContainer.Message($"Мы проводим розыгрыши на каждой трансляции, в случайное время трансляции. Для участия в розыгрыше нужно написать команду{commandText}", commandInfo.Platform);
                }, CommandType.Info);

            return result;
        }

        public Command Participate()
        {
            var command = new Command(_commandName, "Добавляет участника в розыгрыш", Role.Any,
                delegate (CommandInfo commandInfo)
                {
                    var result = string.Empty;
                    var canParticipate = true;
                    var steamTradeLink = _dataBase.GetSteamTradeLink(commandInfo.DisplayName);

                    if (_participantsList.Exists(item => item.Nick == commandInfo.DisplayName))
                    {
                        result = $"{commandInfo.DisplayName} уже участвует в розыгрыше!";
                        canParticipate = false;
                    }

                    if (!commandInfo.ArgumentsAsString.Contains("steamcommunity.com/tradeoffer") && string.IsNullOrEmpty(steamTradeLink))
                    {
                            result = $"{commandInfo.DisplayName} добавь steam-ссылку на торговлю";
                            canParticipate = false;
                    }
                    else if (commandInfo.ArgumentsAsString.Contains("steamcommunity.com/tradeoffer"))
                    {
                        steamTradeLink = commandInfo.ArgumentsAsString;
                        _dataBase.AddSteamTradeLink(commandInfo.DisplayName, steamTradeLink);
                    }

                    var money = _dataBase.GetMoney(commandInfo.DisplayName);
                    if (money < _price)
                    {
                        canParticipate = false;
                        result = $"{commandInfo.DisplayName} у тебя не хватает валюты!";
                    }

                    if (canParticipate)
                    {
                        // if (commandInfo.IsSubscriber.HasValue && commandInfo.IsSubscriber.Value)
                        //     for (var index = 0; index < SubscriberBonus; index++)
                        //         _participantsList.Add(new RaffleParticipant(commandInfo.DisplayName, steamTradeLink));
                        // else
                        _participantsList.Add(new RaffleParticipant(commandInfo.DisplayName, steamTradeLink));
                        _participantsCount++;

                        result = $"{commandInfo.DisplayName} участвует в розыгрыше!";
                    }
                    EventContainer.Message(result, commandInfo.Platform);
                }, CommandType.Hidden);

            return command;
        }

        public RaffleParticipant Giveaway()
        {
            var result = new RaffleParticipant(string.Empty, "Нет участников");
            if (_participantsList.Count > 0)
            {
                var winnerIndex = RandomHelper.Random.Next(0, _participantsList.Count);
                result = _participantsList[winnerIndex];
                EventContainer.Message($"Количество участников: {_participantsCount}!", Platform.Twitch);
                _participantsList.Remove(result);
                IsProgress = false;
                EventContainer.Message($"Победитель розыгрыша: {result.Nick}!", Platform.Twitch);

                foreach (var participant in _participantsList)
                    _dataBase.AddMoney(participant.Nick, -_price);
            }
            return result;
        }
    }
}