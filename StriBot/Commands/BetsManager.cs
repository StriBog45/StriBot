using StriBot.Bots.Enums;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using StriBot.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StriBot.Commands
{
    public class BetsManager
    {
        private bool _betsProcessing;
        private string[] _bettingOptions;
        private int _betsTimer;
        private double _betsCoefficient;
        private readonly Currency _currency;
        public Dictionary<string, (int, int)> UsersBetted { get; set; }

        public BetsManager(Currency currency)
        {
            UsersBetted = new Dictionary<string, (int, int)>();
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
            foreach (var bet in UsersBetted)
            {
                if (bet.Value.Item1 == winner)
                    DataBase.AddMoneyToUser(bet.Key, (int)(bet.Value.Item2 * _betsCoefficient) + (bet.Value.Item2 * (-1)));
                else
                    DataBase.AddMoneyToUser(bet.Key, bet.Value.Item2 * (-1));
            }
            GlobalEventContainer.Message($"Победила ставка под номером {winner}! В ставках участвовало {UsersBetted.Count} енотов! Вы можете проверить свой запас {_currency.GenitiveMultiple}", platforms);

            GlobalEventContainer.Message("Победили: " + UsersBetted.Where(x => x.Value.Item1 == winner)
                .Aggregate(new StringBuilder(), (current, next) => current.Append(current.Length == 0 ? string.Empty : ", ").Append($"{next.Key}:{next.Value.Item2}")).ToString(), platforms);
            GlobalEventContainer.Message("Проиграли: " + UsersBetted.Where(x => x.Value.Item1 != winner)
                .Aggregate(new StringBuilder(), (current, next) => current.Append(current.Length == 0 ? string.Empty : ", ").Append($"{next.Key}:{next.Value.Item2}")).ToString(), platforms);

            UsersBetted.Clear();
            _betsProcessing = false;
        }

        public void CreateBets(string[] options, Platform[] platforms)
        {
            _bettingOptions = new string[options.Length - 1];
            for (int i = 0; i < _bettingOptions.Length; i++)
                _bettingOptions[i] = options[i + 1];
            _betsProcessing = true;
            _betsTimer = 0;
            UsersBetted.Clear();
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
                    int amountOfBets = 0;
                    if (e.ArgumentsAsList.Count == 2 && Int32.TryParse(e.ArgumentsAsList[0], out numberOfBets) && Int32.TryParse(e.ArgumentsAsList[1], out amountOfBets)
                    && numberOfBets < _bettingOptions.Length && amountOfBets > 0)
                    {
                        if (DataBase.CheckMoney(e.DisplayName) < amountOfBets)
                            GlobalEventContainer.Message($"{e.DisplayName} у тебя недостаточно {_currency.GenitiveMultiple} для такой ставки!", platforms);
                        else
                        {
                            if (!UsersBetted.ContainsKey(e.DisplayName))
                            {
                                UsersBetted.Add(e.DisplayName, (numberOfBets, amountOfBets));
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
