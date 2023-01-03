using System.Collections.Generic;
using StriBot.Bots.Enums;
using StriBot.Commands.Enums;
using StriBot.Commands.Extensions;
using StriBot.Commands.Models;
using StriBot.Commands.Raffle.Models;
using StriBot.DateBase.Interfaces;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;

namespace StriBot.Commands.Raffle
{
    public class RaffleManager
    {
        private readonly IDataBase _dataBase;
        private readonly List<RaffleParticipant> _participantsList;
        private string _commandName;
        
        private const int SubscriberBonus = 3;
        private int _participantsCount;

        private bool IsProgress { get; set; }

        public RaffleManager(IDataBase dataBase)
        {
            _dataBase = dataBase;
            _participantsList = new List<RaffleParticipant>();
        }

        public string GetCurrentCommandName()
            => _commandName;

        public void RaffleStart(string name)
        {
            _participantsList.Clear();
            IsProgress = true;
            _commandName = name;
            _participantsCount = 0;
            GlobalEventContainer.Message($"Розыгрыш начался! Пиши команду !{_commandName}", Platform.Twitch);
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
                    GlobalEventContainer.Message($"Мы проводим розыгрыши на каждой трансляции, в случайное время трансляции. Для участия в розыгрыше нужно написать команду{commandText}", commandInfo.Platform);
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

                    if (canParticipate && _participantsList.Exists(item => item.Nick == commandInfo.DisplayName))
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

                    if (canParticipate)
                    {
                        if (commandInfo.IsSubscriber.HasValue && commandInfo.IsSubscriber.Value)
                            for (var index = 0; index < SubscriberBonus; index++)
                                _participantsList.Add(new RaffleParticipant(commandInfo.DisplayName, steamTradeLink));
                        else
                            _participantsList.Add(new RaffleParticipant(commandInfo.DisplayName, steamTradeLink));
                        _participantsCount++;

                        result = $"{commandInfo.DisplayName} участвует в розыгрыше!";
                    }
                    GlobalEventContainer.Message(result, commandInfo.Platform);
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
                GlobalEventContainer.Message($"Количество участников: {_participantsCount}!", Platform.Twitch);
                _participantsList.Remove(result);
                IsProgress = false;
                GlobalEventContainer.Message($"Победитель розыгрыша: {result.Nick}!", Platform.Twitch);
            }
            return result;
        }
    }
}