using StriBot.Bots.Enums;
using StriBot.Commands.Extensions;
using StriBot.Commands.Raffle.Models;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using System.Collections.Generic;

namespace StriBot.Commands.Raffle
{
    public class RaffleManager
    {
        private List<RaffleParticipant> _participantsList;
        private string _commandName;
        
        private const int _subscriberBonus = 3;

        public bool IsProgress { get; private set; }

        public RaffleManager()
        {
            _participantsList = new List<RaffleParticipant>();
        }

        public string GetCurrentCommandName()
            => _commandName;

        public void RaffleStart(string name)
        {
            _participantsList.Clear();
            if (!string.IsNullOrEmpty(name) && name != "!")
            {
                if (name[0] == '!')
                    name = name.Replace("!", string.Empty);
                IsProgress = true;
                _commandName = name;
                GlobalEventContainer.Message($"Розыгрыш начался! Пиши команду !{_commandName}", Platform.Twitch);
            }
        }

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>()
            {
                RaffleInfo()
            };

        public Command RaffleInfo()
        {
            var result = new Command("розыгрыш", "информация по розыгрышу", Role.Any,
                delegate (CommandInfo commandInfo)
                {
                    var commandText = IsProgress
                    ? "которую укажет стример"
                    : $"!{_commandName}";
                    GlobalEventContainer.Message($"Мы проводим розыгрыши на каждой трансляции, в случайное время трансляции. Для участия в розыгрыше нужно написать команду {commandText}", commandInfo.Platform);
                }, CommandType.Info);

            return result;
        }

        public Command Participate()
        {
            var result = new Command(_commandName, "Добавляет участника в розыгрыш", Role.Any,
                delegate (CommandInfo commandInfo)
                {
                    if (!string.IsNullOrEmpty(commandInfo.ArgumentsAsString) && commandInfo.ArgumentsAsString.Contains("steamcommunity.com/tradeoffer"))
                    {
                        if (!_participantsList.Exists(item => item.Nick == commandInfo.DisplayName))
                        {
                            if (commandInfo.IsSubscriber.HasValue && commandInfo.IsSubscriber.Value)
                            {
                                for (var index = 0; index < _subscriberBonus; index++)
                                    _participantsList.Add(new RaffleParticipant(commandInfo.DisplayName, commandInfo.ArgumentsAsString));
                            }
                            else
                                _participantsList.Add(new RaffleParticipant(commandInfo.DisplayName, commandInfo.ArgumentsAsString));
                            GlobalEventContainer.Message($"{commandInfo.DisplayName} участвует в розыгрыше!", commandInfo.Platform);
                        }
                        else
                            GlobalEventContainer.Message($"{commandInfo.DisplayName} уже участвует в розыгрыше!", commandInfo.Platform);
                    }
                    else
                        GlobalEventContainer.Message($"{commandInfo.DisplayName} добавь steam-ссылку на торговлю", commandInfo.Platform);
                }, CommandType.Hidden);

            return result;
        }

        public RaffleParticipant Giveaway()
        {
            var result = new RaffleParticipant(string.Empty, "Нет участников");
            if (_participantsList.Count > 0)
            {
                var winnerIndex = RandomHelper.random.Next(0, _participantsList.Count);
                result = _participantsList[winnerIndex];
                _participantsList.Remove(result);
                IsProgress = false;
                GlobalEventContainer.Message($"Победитель розыгрыша: {result.Nick}!", Platform.Twitch);
            }
            return result;
        }
    }
}