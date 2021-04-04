using StriBot.Bots.Enums;
using StriBot.Commands.Extensions;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using System.Collections.Generic;

namespace StriBot.Commands
{
    public class RaffleManager
    {
        private string _commandName;
        private List<(string Nickname, string Link)> _participantsList;

        private const int _subscriberBonus = 3;

        public RaffleManager()
        {
            _participantsList = new List<(string Nickname, string Link)>();
        }

        public string GetCurrentCommandName()
            => _commandName;

        public void ChangeCommandName(string name)
        {
            _commandName = name;
            _participantsList.Clear();
        }

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>()
            {
                RaffleInfo()
            };

        public Command RaffleInfo()
        {
            var result = new Command("розыгрыш", "информация по розыгрышу", Role.Moderator,
                delegate (CommandInfo commandInfo)
                {
                    var commandText = string.IsNullOrEmpty(_commandName)
                    ? "которую укажет стример"
                    : $"!{_commandName}";
                    GlobalEventContainer.Message($"Мы проводим розыгрыши на каждой трансляции, в случайное время трансляции. Для участия в розыгрыше нужно написать команду {commandText}", commandInfo.Platform);
                }, CommandType.Info);

            return result;
        }

        public Command Participate()
        {
            var result = new Command(_commandName, "Добавляет участника в розыгрыш", Role.Moderator,
                delegate (CommandInfo commandInfo)
                {
                    if (!string.IsNullOrEmpty(commandInfo.ArgumentsAsString) && commandInfo.ArgumentsAsString.Contains("steamcommunity.com/tradeoffer"))
                    {
                        if (!_participantsList.Exists(item => item.Nickname == commandInfo.DisplayName))
                        {
                            if (commandInfo.IsSubscriber.HasValue && commandInfo.IsSubscriber.Value)
                            {
                                for (var index = 0; index < _subscriberBonus; index++)
                                    _participantsList.Add((commandInfo.DisplayName, commandInfo.ArgumentsAsString));
                            }
                            else
                                _participantsList.Add((commandInfo.DisplayName, commandInfo.ArgumentsAsString));
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

        public (string Nickname, string Link) Giveaway()
        {
            (string Nickname, string Link) result = (string.Empty, "Нет участников");
            if (_participantsList.Count > 0)
            {
                var winnerIndex = RandomHelper.random.Next(0, _participantsList.Count);
                result = _participantsList[winnerIndex];
                GlobalEventContainer.Message($"Победитель розыгрыша: {result.Nickname}!", Platform.Twitch);
                _participantsList.Clear();
            }
            return result;
        }
    }
}