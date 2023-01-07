using System.Collections.Generic;
using StriBot.Application.Commands.Enums;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;
using StriBot.Application.Extensions;
using StriBot.Application.Localization.Extensions;
using StriBot.Application.Localization.Implementations;
using StriBot.Commands.CommonFunctions;
using StriBot.Commands.Extensions;
using StriBot.Commands.Models;

namespace StriBot.Commands
{
    public class DuelManager
    {
        private readonly Currency _currency;
        private readonly ReadyMadePhrases _readyMadePhrases;
        private readonly IDataBase _dataBase;

        private bool _isDuelActive;
        private int _duelTimer;
        private int _duelBet;
        private CommandInfo _duelMember;
        private const int TimeoutTime = 120;

        public DuelManager(Currency currency, ReadyMadePhrases readyMadePhrases, IDataBase dataBase)
        {
            _currency = currency;
            _readyMadePhrases = readyMadePhrases;
            _dataBase = dataBase;

            CleanDuelMember();
        }

        private Command CreateDuelCommand()
        {
            var result = new Command("Дуэль", $"Дуэль с {_currency.InstrumentalMultiple} или без, с timeout, проигравший в дуэли отправляется на {TimeoutTime} секунд в timeout",
                delegate (CommandInfo commandInfo)
                {
                    if (!_isDuelActive)
                    {
                        if (commandInfo.ArgumentsAsList.Count > 0)
                        {
                            if (int.TryParse(commandInfo.ArgumentsAsString, out int amount) && amount > 0)
                            {
                                if (amount <= _dataBase.GetMoney(commandInfo.DisplayName))
                                {
                                    EventContainer.Message($"Кто осмелится принять вызов {commandInfo.DisplayName} в смертельной дуэли со ставкой в {_currency.Incline(amount, true)}?", 
                                        commandInfo.Platform);
                                    StartDuel(commandInfo,  amount);
                                }
                                else
                                    _readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
                            }
                            else
                                ReadyMadePhrases.IncorrectCommand(commandInfo.Platform);
                        }
                        else
                        {
                            EventContainer.Message($"Кто осмелится принять вызов {commandInfo.DisplayName} в смертельной дуэли?", commandInfo.Platform);
                            StartDuel(commandInfo, 0);
                        }
                    }
                    else
                    {
                        if (_duelMember.DisplayName == commandInfo.DisplayName)
                            EventContainer.Message($"@{_duelMember.DisplayName} не торопись! Твоё время ещё не пришло", commandInfo.Platform);
                        else if (_dataBase.GetMoney(commandInfo.DisplayName) < _duelBet)
                            _readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
                        else
                        {
                            EventContainer.Message($"Смертельная дуэль между {_duelMember.DisplayName} и {commandInfo.DisplayName}!", commandInfo.Platform);
                            var winner = _duelMember;
                            var looser = _duelMember;
                            if (RandomHelper.Random.Next(2) == 0)
                                winner = commandInfo;
                            else
                                looser = commandInfo;
                            _dataBase.AddMoney(winner.DisplayName, _duelBet);
                            _dataBase.AddMoney(looser.DisplayName, -_duelBet);

                            if (_duelBet > 0)
                                EventContainer.Message($"Дуэлянты достают пистолеты... Выстрел!.. На земле лежит {looser.DisplayName}. {winner.DisplayName} получил за победу {_duelBet} {_currency.GenitiveMultiple}! Kappa )/",
                                    commandInfo.Platform);
                            else
                                EventContainer.Message($"Дуэлянты достают пистолеты... Выстрел!.. На земле лежит {looser.DisplayName}. Поздравляем победителя {winner.DisplayName} Kappa )/", commandInfo.Platform);
#warning Нужно событие timeout
                            //if (looser.IsModerator.HasValue && !looser.IsModerator.Value)
                            //    twitchBot.UserTimeout(looser.DisplayName, new TimeSpan(0, timeoutTimeInMinute, 0), "Ваш противник - (凸ಠ益ಠ)凸");
                            CleanDuelMember();
                        }
                    }
                }, new[] { "размер ставки" }, CommandType.Interactive);

            return result;
        }

        private void StartDuel(CommandInfo commandInfo, int bet)
        {
            _duelMember = commandInfo;
            _duelTimer = 0;
            _duelBet = bet;
            _isDuelActive = true;
        }

        private void CleanDuelMember()
        {
            _duelBet = 0;
            _duelMember = null;
            _isDuelActive = false;
        }

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>()
            { 
                CreateDuelCommand()
            };

        public void Tick()
        {
            if (_isDuelActive)
                if (_duelTimer >= 3)
                {
                    EventContainer.Message($"Дуэль {_duelMember.DisplayName} никто не принял", _duelMember.Platform);
                    _duelTimer = 0;
                    CleanDuelMember();
                }
                else
                    _duelTimer++;
        }
    }
}