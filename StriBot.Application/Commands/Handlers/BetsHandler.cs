using System.Collections.Generic;
using System.Linq;
using System.Text;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Commands.Extensions;
using StriBot.Application.Commands.Models;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;
using StriBot.Application.Localization.Implementations;
using StriBot.Application.Platforms.Enums;

namespace StriBot.Application.Commands.Handlers
{
    public class BetsHandler
    {
        private readonly Currency _currency;
        private readonly IDataBase _dataBase;

        private bool _betsProcessing;
        private string[] _bettingOptions;
        private int _betsTimer;
        private double _betsCoefficient;
        private readonly Dictionary<string, (int Choice, int BetSize)> _usersBid;

        public BetsHandler(Currency currency, IDataBase dataBase)
        {
            _usersBid = new Dictionary<string, (int, int)>();
            _currency = currency;
            _dataBase = dataBase;
        }

        public void Tick(Platform[] platforms)
        {
            if (_betsProcessing)
            {
                _betsTimer++;
                if (_betsTimer == 5)
                    StopBetsProcess(platforms);
            };
        }

        public void StopBetsProcess(Platform[] platforms)
        {
            _betsProcessing = false;
            _betsTimer = 0;
            EventContainer.Message("Ставки больше не принимаются", platforms);
        }

        public void SetBetsWinner(int winner, Platform[] platforms)
        {
            if (_betsProcessing)
            {
                foreach (var bet in _usersBid)
                {
                    if (bet.Value.Choice == winner)
                        _dataBase.AddMoney(bet.Key, (int)(bet.Value.BetSize * _betsCoefficient) + (bet.Value.BetSize * (-1)));
                    else
                        _dataBase.AddMoney(bet.Key, bet.Value.BetSize * (-1));
                }
                EventContainer.Message($"Победила ставка под номером {winner} - {_bettingOptions[winner]}! В ставках участвовало {_usersBid.Count} енотов! Вы можете проверить свой запас {_currency.GenitiveMultiple}", platforms);

                var winners = _usersBid.Where(x => x.Value.Choice == winner)
                    .Aggregate(new StringBuilder(), (current, next) => current.Append(current.Length == 0 ? string.Empty : ", ").Append($"{next.Key}:{next.Value.BetSize}")).ToString();
                var losers = _usersBid.Where(x => x.Value.Choice != winner)
                    .Aggregate(new StringBuilder(), (current, next) => current.Append(current.Length == 0 ? string.Empty : ", ").Append($"{next.Key}:{next.Value.BetSize}")).ToString();

                if (!string.IsNullOrEmpty(winners))
                    EventContainer.Message("Победили: " + winners, platforms);
                else
                    EventContainer.Message("Победителей нет", platforms);

                if (!string.IsNullOrEmpty(losers))
                    EventContainer.Message("Проиграли: " + losers, platforms);
                else
                    EventContainer.Message("Проигравших нет", platforms);

                _usersBid.Clear();
                _betsProcessing = false;
            }
        }

        public void CreateBets(string[] options, Platform[] platforms)
        {
            _bettingOptions = new string[options.Length^1];
            for (var i = 0; i < _bettingOptions.Length; i++)
                _bettingOptions[i] = options[i + 1];
            _betsProcessing = true;
            _betsTimer = 0;
            _usersBid.Clear();
            _betsCoefficient = (options.Length * 0.5);

            EventContainer.Message($"Время ставок! Коэффициент {_betsCoefficient}. Для участия необходимо написать !ставка [номер ставки] [сколько ставите]", platforms);

            var messageBuilder = new StringBuilder($"{options[0]}: ");
            for (var i = 0; i < _bettingOptions.Length; i++)
            {
                if (i != 0)
                    messageBuilder.Append(", ");
                messageBuilder.Append($"{i} - {_bettingOptions[i]}");
            }
            EventContainer.Message(messageBuilder.ToString(), platforms);
        }

        private Command CreateBetCommand(Platform[] platforms)
        {
            void Action(CommandInfo commandInfo)
            {
                if (_betsProcessing)
                {
                    if (commandInfo.ArgumentsAsList.Count == 2 && int.TryParse(commandInfo.ArgumentsAsList[0], out var numberOfBets) && int.TryParse(commandInfo.ArgumentsAsList[1], out var betSize) && numberOfBets < _bettingOptions.Length && betSize > 0)
                    {
                        if (_dataBase.GetMoney(commandInfo.DisplayName) < betSize)
                            EventContainer.Message($"{commandInfo.DisplayName} у тебя недостаточно {_currency.GenitiveMultiple} для такой ставки!", platforms);
                        else
                        {
                            if (!_usersBid.ContainsKey(commandInfo.DisplayName))
                            {
                                _usersBid.Add(commandInfo.DisplayName, (numberOfBets, betSize));
                                EventContainer.Message($"{commandInfo.DisplayName} успешно сделал ставку!", platforms);
                            }
                            else
                                EventContainer.Message($"{commandInfo.DisplayName} уже сделал ставку!", platforms);
                        }
                    }
                    else
                        EventContainer.Message($"{commandInfo.DisplayName} вы неправильно указали ставку", platforms);
                }
                else
                    EventContainer.Message("В данный момент ставить нельзя!", platforms);
            }

            return new Command("Ставка", "Сделать ставку", Action, new[] { "на что", "сколько" }, CommandType.Interactive);
        }

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>
            {
                CreateBetCommand(new[] {Platform.Twitch})
            };
    }
}