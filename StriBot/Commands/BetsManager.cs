using StriBot.Bots.Enums;
using StriBot.Commands.Enums;
using StriBot.Commands.Extensions;
using StriBot.Commands.Models;
using StriBot.DateBase;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using StriBot.Language.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StriBot.Commands
{
    public class BetsManager
    {
        private readonly Currency _currency;

        private bool _betsProcessing;
        private string[] _bettingOptions;
        private int _betsTimer;
        private double _betsCoefficient;
        private Dictionary<string, (int Choice, int BetSize)> _usersBetted;

        public BetsManager(Currency currency)
        {
            _usersBetted = new Dictionary<string, (int, int)>();
            _currency = currency;
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
            GlobalEventContainer.Message("Ставки больше не принимаются", platforms);
        }

        public void SetBetsWinner(int winner, Platform[] platforms)
        {
            if (_betsProcessing)
            {
                foreach (var bet in _usersBetted)
                {
                    if (bet.Value.Choice == winner)
                        DataBase.AddMoney(bet.Key, (int)(bet.Value.BetSize * _betsCoefficient) + (bet.Value.BetSize * (-1)));
                    else
                        DataBase.AddMoney(bet.Key, bet.Value.BetSize * (-1));
                }
                GlobalEventContainer.Message($"Победила ставка под номером {winner} - {_bettingOptions[winner]}! В ставках участвовало {_usersBetted.Count} енотов! Вы можете проверить свой запас {_currency.GenitiveMultiple}", platforms);

                var winers = _usersBetted.Where(x => x.Value.Choice == winner)
                    .Aggregate(new StringBuilder(), (current, next) => current.Append(current.Length == 0 ? string.Empty : ", ").Append($"{next.Key}:{next.Value.BetSize}")).ToString();
                var loosers = _usersBetted.Where(x => x.Value.Choice != winner)
                    .Aggregate(new StringBuilder(), (current, next) => current.Append(current.Length == 0 ? string.Empty : ", ").Append($"{next.Key}:{next.Value.BetSize}")).ToString();

                if (!string.IsNullOrEmpty(winers))
                    GlobalEventContainer.Message("Победили: " + winers, platforms);
                else
                    GlobalEventContainer.Message("Победителей нет", platforms);

                if (!string.IsNullOrEmpty(loosers))
                    GlobalEventContainer.Message("Проиграли: " + loosers, platforms);
                else
                    GlobalEventContainer.Message("Проигравших нет", platforms);

                _usersBetted.Clear();
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
            _usersBetted.Clear();
            _betsCoefficient = (options.Length * 0.5);

            GlobalEventContainer.Message($"Время ставок! Коэффициент {_betsCoefficient}. Для участия необходимо написать !ставка [номер ставки] [сколько ставите]", platforms);

            var messageBuilder = new StringBuilder($"{options[0]}: ");
            for (var i = 0; i < _bettingOptions.Length; i++)
            {
                if (i != 0)
                    messageBuilder.Append(", ");
                messageBuilder.Append($"{i} - {_bettingOptions[i]}");
            }
            GlobalEventContainer.Message(messageBuilder.ToString(), platforms);
        }

        private Command CreateBetCommand(Platform[] platforms)
        {
            void Action(CommandInfo commandInfo)
            {
                if (_betsProcessing)
                {
                    if (commandInfo.ArgumentsAsList.Count == 2 && int.TryParse(commandInfo.ArgumentsAsList[0], out var numberOfBets) && int.TryParse(commandInfo.ArgumentsAsList[1], out var betSize) && numberOfBets < _bettingOptions.Length && betSize > 0)
                    {
                        if (DataBase.GetMoney(commandInfo.DisplayName) < betSize)
                            GlobalEventContainer.Message($"{commandInfo.DisplayName} у тебя недостаточно {_currency.GenitiveMultiple} для такой ставки!", platforms);
                        else
                        {
                            if (!_usersBetted.ContainsKey(commandInfo.DisplayName))
                            {
                                _usersBetted.Add(commandInfo.DisplayName, (numberOfBets, betSize));
                                GlobalEventContainer.Message($"{commandInfo.DisplayName} успешно сделал ставку!", platforms);
                            }
                            else
                                GlobalEventContainer.Message($"{commandInfo.DisplayName} уже сделал ставку!", platforms);
                        }
                    }
                    else
                        GlobalEventContainer.Message($"{commandInfo.DisplayName} вы неправильно указали ставку", platforms);
                }
                else
                    GlobalEventContainer.Message("В данный момент ставить нельзя!", platforms);
            }

            return new Command("Ставка", "Сделать ставку", Action, new[] { "на что", "сколько" }, CommandType.Interactive);
        }

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>()
            {
                CreateBetCommand(new[] {Platform.Twitch})
            };
    }
}