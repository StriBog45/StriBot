using StriBot.Commands.CommonFunctions;
using StriBot.Commands.Enums;
using StriBot.Commands.Extensions;
using StriBot.Commands.Models;
using StriBot.DateBase;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using StriBot.Language.Extensions;
using StriBot.Language.Implementations;
using System;
using System.Collections.Generic;

namespace StriBot.Commands
{
    public class DuelManager
    {
        private Currency _currency;
        private ReadyMadePhrases _readyMadePhrases;

        private bool _isDuelActive;
        private int _duelTimer;
        private int _duelBet;
        private CommandInfo _duelMember;
        private int _timeoutTime = 120;

        public DuelManager(Currency currency, ReadyMadePhrases readyMadePhrases)
        {
            _currency = currency;
            _readyMadePhrases = readyMadePhrases;

            CleanDuelMember();
        }

        public Command CreateDuelCommand()
        {
            var result = new Command("Дуэль", $"Дуэль с {_currency.InstrumentalMultiple} или без, с timeout, проигравший в дуэли отправляется на {_timeoutTime} секунд в timeout",
                delegate (CommandInfo commandInfo)
                {
                    if (!_isDuelActive)
                    {
                        if (commandInfo.ArgumentsAsList.Count > 0)
                        {
                            if (int.TryParse(commandInfo.ArgumentsAsString, out int amount) && amount > 0)
                            {
                                if (amount <= DataBase.GetMoney(commandInfo.DisplayName))
                                {
                                    GlobalEventContainer.Message($"Кто осмелится принять вызов {commandInfo.DisplayName} в смертельной дуэли со ставкой в {_currency.Incline(amount, true)}?", 
                                        commandInfo.Platform);
                                    StartDuel(commandInfo,  amount);
                                }
                                else
                                    _readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
                            }
                            else
                                _readyMadePhrases.IncorrectCommand(commandInfo.Platform);
                        }
                        else
                        {
                            GlobalEventContainer.Message($"Кто осмелится принять вызов {commandInfo.DisplayName} в смертельной дуэли?", commandInfo.Platform);
                            StartDuel(commandInfo, 0);
                        }
                    }
                    else
                    {
                        if (_duelMember.DisplayName == commandInfo.DisplayName)
                            GlobalEventContainer.Message($"@{_duelMember.DisplayName} не торопись! Твоё время ещё не пришло", commandInfo.Platform);
                        else if (DataBase.GetMoney(commandInfo.DisplayName) < _duelBet)
                            _readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
                        else
                        {
                            GlobalEventContainer.Message($"Смертельная дуэль между {_duelMember.DisplayName} и {commandInfo.DisplayName}!", commandInfo.Platform);
                            var winner = _duelMember;
                            var looser = _duelMember;
                            if (RandomHelper.random.Next(2) == 0)
                                winner = commandInfo;
                            else
                                looser = commandInfo;
                            DataBase.AddMoney(winner.DisplayName, _duelBet);
                            DataBase.AddMoney(looser.DisplayName, -_duelBet);

                            if (_duelBet > 0)
                                GlobalEventContainer.Message($"Дуэлянты достают пистолеты... Выстрел!.. На земле лежит {looser.DisplayName}. {winner.DisplayName} получил за победу {_duelBet} {_currency.GenitiveMultiple}! Kappa )/",
                                    commandInfo.Platform);
                            else
                                GlobalEventContainer.Message($"Дуэлянты достают пистолеты... Выстрел!.. На земле лежит {looser.DisplayName}. Поздравляем победителя {winner.DisplayName} Kappa )/", commandInfo.Platform);
#warning Нужно событие timeout
                            //if (looser.IsModerator.HasValue && !looser.IsModerator.Value)
                            //    twitchBot.UserTimeout(looser.DisplayName, new TimeSpan(0, timeoutTimeInMinute, 0), "Ваш противник - (凸ಠ益ಠ)凸");
                            CleanDuelMember();
                        }
                    }
                }, new string[] { "размер ставки" }, CommandType.Interactive);

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
                    GlobalEventContainer.Message($"Дуэль {_duelMember.DisplayName} никто не принял", _duelMember.Platform);
                    _duelTimer = 0;
                    CleanDuelMember();
                }
                else
                    _duelTimer++;
        }
    }
}