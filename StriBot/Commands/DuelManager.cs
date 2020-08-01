using StriBot.Commands.CommonFunctions;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using StriBot.Language;
using System;
using System.Collections.Generic;

namespace StriBot.Commands
{
    public class DuelManager
    {
        private Currency currency;
        private ReadyMadePhrases readyMadePhrases;

        private bool isDuelActive;
        private int duelTimer;
        private int duelBet;
        private CommandInfo duelMember;
        private int timeoutTime = 120;
        private int timeoutTimeInMinute = 2;

        public DuelManager(Currency currency, ReadyMadePhrases readyMadePhrases)
        {
            this.currency = currency;
            this.readyMadePhrases = readyMadePhrases;

            CleanDuelMember();
        }

        public Command CreateDuelCommand()
        {
            var result = new Command("Дуэль", $"Дуэль с {currency.InstrumentalMultiple} или без, с timeout, проигравший в дуэли отправляется на {timeoutTime} секунд в timeout",
                delegate (CommandInfo commandInfo)
                {
                    if (!isDuelActive)
                    {
                        if (commandInfo.ArgumentsAsList.Count > 0)
                        {
                            if (Int32.TryParse(commandInfo.ArgumentsAsString, out int amount) && amount > 0)
                            {
                                if (amount <= DataBase.CheckMoney(commandInfo.DisplayName))
                                {
                                    GlobalEventContainer.Message($"Кто осмелится принять вызов {commandInfo.DisplayName} в смертельной дуэли со ставкой в {currency.Incline(amount, true)}?", 
                                        commandInfo.Platform);
                                    StartDuel(commandInfo,  amount);
                                }
                                else
                                    readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
                            }
                            else
                                readyMadePhrases.IncorrectCommand(commandInfo.Platform);
                        }
                        else
                        {
                            GlobalEventContainer.Message($"Кто осмелится принять вызов {commandInfo.DisplayName} в смертельной дуэли?", commandInfo.Platform);
                            StartDuel(commandInfo, 0);
                        }
                    }
                    else
                    {
                        if (duelMember.DisplayName == commandInfo.DisplayName)
                            GlobalEventContainer.Message($"@{duelMember.DisplayName} не торопись! Твоё время ещё не пришло", commandInfo.Platform);
                        else if (DataBase.CheckMoney(commandInfo.DisplayName) < duelBet)
                            readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
                        else
                        {
                            GlobalEventContainer.Message($"Смертельная дуэль между {duelMember.DisplayName} и {commandInfo.DisplayName}!", commandInfo.Platform);
                            var winner = duelMember;
                            var looser = duelMember;
                            if (RandomHelper.random.Next(2) == 0)
                                winner = commandInfo;
                            else
                                looser = commandInfo;
                            DataBase.AddMoneyToUser(winner.DisplayName, duelBet);
                            DataBase.AddMoneyToUser(looser.DisplayName, -duelBet);

                            if (duelBet > 0)
                                GlobalEventContainer.Message($"Дуэлянты достают пистолеты... Выстрел!.. На земле лежит {looser.DisplayName}. {winner.DisplayName} получил за победу {duelBet} {currency.GenitiveMultiple}! Kappa )/",
                                    commandInfo.Platform);
                            else
                                GlobalEventContainer.Message($"Дуэлянты достают пистолеты... Выстрел!.. На земле лежит {looser.DisplayName}. Поздравляем победителя {winner.DisplayName} Kappa )/", commandInfo.Platform);
                            //if (!looser.IsModerator)
                            //    twitchBot.UserTimeout(looser.DisplayName, new TimeSpan(0, timeoutTimeInMinute, 0), "Ваш противник - (凸ಠ益ಠ)凸");
                            CleanDuelMember();
                        }
                    }
                }, new string[] { "размер ставки" }, CommandType.Interactive);

            return result;
        }

        private void StartDuel(CommandInfo commandInfo, int bet)
        {
            duelMember = commandInfo;
            duelTimer = 0;
            duelBet = bet;
            isDuelActive = true;
        }

        private void CleanDuelMember()
        {
            duelBet = 0;
            duelMember = null;
            isDuelActive = false;
        }

        public Dictionary<string, Command> CreateCommands()
        {
            var result = new Dictionary<string, Command>();
            result.Add(CreateDuelCommand());

            return result;
        }

        public void Tick()
        {
            if (isDuelActive)
                if (duelTimer >= 3)
                {
                    GlobalEventContainer.Message($"Дуэль {duelMember.DisplayName} никто не принял", duelMember.Platform);
                    duelTimer = 0;
                    CleanDuelMember();
                }
                else
                    duelTimer++;
        }
    }
}