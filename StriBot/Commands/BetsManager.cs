using StriBot.Bots.Enums;
using StriBot.Commands.Extensions;
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
            foreach (var bet in _usersBetted)
            {
                if (bet.Value.Choice == winner)
                    DataBase.AddMoneyToUser(bet.Key, (int)(bet.Value.BetSize * _betsCoefficient) + (bet.Value.BetSize * (-1)));
                else
                    DataBase.AddMoneyToUser(bet.Key, bet.Value.BetSize * (-1));
            }
            GlobalEventContainer.Message($"Победила ставка под номером {winner}! В ставках участвовало {_usersBetted.Count} енотов! Вы можете проверить свой запас {_currency.GenitiveMultiple}", platforms);

            var winers = _usersBetted.Where(x => x.Value.Choice == winner)
                .Aggregate(new StringBuilder(), (current, next) => current.Append(current.Length == 0 ? string.Empty : ", ").Append($"{next.Key}:{next.Value.BetSize}")).ToString();
            var loosers = _usersBetted.Where(x => x.Value.Choice != winner)
                .Aggregate(new StringBuilder(), (current, next) => current.Append(current.Length == 0 ? string.Empty : ", ").Append($"{next.Key}:{next.Value.BetSize}")).ToString();

            if (!string.IsNullOrEmpty(winers))
                GlobalEventContainer.Message("Победили: " + winers, platforms);
            else
                GlobalEventContainer.Message("Победителей нет", platforms);

            if (!string.IsNullOrEmpty(loosers))
                GlobalEventContainer.Message("Проиграли: " +  loosers, platforms);
            else
                GlobalEventContainer.Message("Проигравших нет", platforms);

            _usersBetted.Clear();
            _betsProcessing = false;
        }

        public void CreateBets(string[] options, Platform[] platforms)
        {
            _bettingOptions = new string[options.Length - 1];
            for (int i = 0; i < _bettingOptions.Length; i++)
                _bettingOptions[i] = options[i + 1];
            _betsProcessing = true;
            _betsTimer = 0;
            _usersBetted.Clear();
            _betsCoefficient = (options.Length * 0.5);

            GlobalEventContainer.Message(string.Format("Время ставок! Коэффициент {0}. Для участия необходимо написать !ставка [номер ставки] [сколько ставите]", _betsCoefficient), platforms);

            StringBuilder messageBuilder = new StringBuilder(string.Format("{0}: ", options[0]));
            for (int i = 0; i < _bettingOptions.Length; i++)
            {
                if (i != 0)
                    messageBuilder.Append(", ");
                messageBuilder.Append($"{i} - {_bettingOptions[i]}");
            }
            GlobalEventContainer.Message(messageBuilder.ToString(), platforms);
        }

        public Command CreateBetCommand(Platform[] platforms)
        {
            Action<CommandInfo> action = delegate (CommandInfo e) {
                if (_betsProcessing)
                {
                    int numberOfBets = 0;
                    int betSize = 0;
                    if (e.ArgumentsAsList.Count == 2 && Int32.TryParse(e.ArgumentsAsList[0], out numberOfBets) && Int32.TryParse(e.ArgumentsAsList[1], out betSize)
                    && numberOfBets < _bettingOptions.Length && betSize > 0)
                    {
                        if (DataBase.CheckMoney(e.DisplayName) < betSize)
                            GlobalEventContainer.Message($"{e.DisplayName} у тебя недостаточно {_currency.GenitiveMultiple} для такой ставки!", platforms);
                        else
                        {
                            if (!_usersBetted.ContainsKey(e.DisplayName))
                            {
                                _usersBetted.Add(e.DisplayName, (numberOfBets, betSize));
                                GlobalEventContainer.Message($"{e.DisplayName} успешно сделал ставку!", platforms);
                            }
                            else
                                GlobalEventContainer.Message($"{e.DisplayName} уже сделал ставку!", platforms);
                        }
                    }
                    else
                        GlobalEventContainer.Message($"{e.DisplayName} вы неправильно указали ставку", platforms);
                }
                else
                    GlobalEventContainer.Message("В данный момент ставить нельзя!", platforms);
            };

            return new Command("Ставка", "Сделать ставку", action, new string[] { "на что", "сколько" }, CommandType.Interactive);
        }

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>()
            {
                CreateBetCommand(new Platform[]{Platform.Twitch })
            };
    }
}