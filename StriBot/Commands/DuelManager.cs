using StriBot.Commands.CommonFunctions;
using StriBot.Language;
using StriBot.TwitchBot.Interfaces;
using System;
using System.Collections.Generic;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace StriBot.Commands
{
    public class DuelManager
    {
        private Currency currency;
        private ITwitchBot twitchBot;
        private ReadyMadePhrases readyMadePhrases;

        private int duelTimer;
        private Tuple<ChatMessage, int> duelMember;
        private int timeoutTime = 120;
        private int timeoutTimeInMinute = 2;

        public DuelManager(Currency currency, ITwitchBot twitchBot, ReadyMadePhrases readyMadePhrases)
        {
            this.currency = currency;
            this.twitchBot = twitchBot;
            this.readyMadePhrases = readyMadePhrases;
        }

        public Command CreateDuelCommand()
        {
            var result = new Command("Дуэль", $"Дуэль с {currency.InstrumentalMultiple} или без, с timeout, проигравший в дуэли отправляется на {timeoutTime} секунд в timeout",
                delegate (OnChatCommandReceivedArgs e)
                {
                    int amount = 0;
                    if (duelMember == null)
                    {
                        if (e.Command.ArgumentsAsList.Count > 0)
                        {
                            Int32.TryParse(e.Command.ArgumentsAsString, out amount);
                            if (amount > 0)
                            {
                                if (amount <= DataBase.CheckMoney(e.Command.ChatMessage.DisplayName))
                                {
                                    twitchBot.SendMessage($"Кто осмелится принять вызов {e.Command.ChatMessage.DisplayName} в смертельной дуэли со ставкой в {amount} {currency.Incline(amount, true)}?");
                                    duelMember = new Tuple<ChatMessage, int>(e.Command.ChatMessage, amount);
                                    duelTimer = 0;
                                }
                                else
                                    readyMadePhrases.NoMoney(e.Command.ChatMessage.DisplayName);
                            }
                            else
                                readyMadePhrases.IncorrectCommand();
                        }
                        else
                        {
                            twitchBot.SendMessage($"Кто осмелится принять вызов {e.Command.ChatMessage.DisplayName} в смертельной дуэли?");
                            duelMember = new Tuple<ChatMessage, int>(e.Command.ChatMessage, amount);
                            duelTimer = 0;
                        }

                    }
                    else
                    {
                        if (duelMember.Item1.DisplayName == e.Command.ChatMessage.DisplayName)
                            twitchBot.SendMessage($"@{duelMember.Item1.DisplayName} не торопись! Твоё время ещё не пришло");
                        else if (DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) < duelMember.Item2)
                            readyMadePhrases.NoMoney(e.Command.ChatMessage.DisplayName);
                        else
                        {
                            twitchBot.SendMessage($"Смертельная дуэль между {duelMember.Item1.DisplayName} и {e.Command.ChatMessage.DisplayName}!");
                            ChatMessage winner = duelMember.Item1;
                            ChatMessage looser = duelMember.Item1;
                            if (RandomHelper.random.Next(2) == 0)
                                winner = e.Command.ChatMessage;
                            else
                                looser = e.Command.ChatMessage;
                            DataBase.AddMoneyToUser(winner.DisplayName, duelMember.Item2);
                            DataBase.AddMoneyToUser(looser.DisplayName, -duelMember.Item2);
                            if (amount != 0)
                                twitchBot.SendMessage($"Дуэлянты достают пистолеты... Выстрел!.. На земле лежит {looser.DisplayName}. {winner.DisplayName} получил за победу {duelMember.Item2} {currency.GenitiveMultiple}! Kappa )/");
                            else
                                twitchBot.SendMessage($"Дуэлянты достают пистолеты... Выстрел!.. На земле лежит {looser.DisplayName}. Поздравляем победителя {winner.DisplayName} Kappa )/");
                            if (!looser.IsModerator)
                                twitchBot.UserTimeout(looser.Username, new TimeSpan(0, timeoutTimeInMinute, 0), "Ваш противник - (凸ಠ益ಠ)凸");
                            duelMember = null;
                        }
                    }
                }, new string[] { "размер ставки" }, CommandType.Interactive);

            return result;
        }

        public Dictionary<string, Command> CreateCommands()
        {
            var result = new Dictionary<string, Command>();
            result.Add(CreateDuelCommand());

            return result;
        }

        public void Tick()
        {
            if (duelMember != null)
                if (duelTimer >= 3)
                {
                    twitchBot.SendMessage($"Дуэль {duelMember.Item1.DisplayName} никто не принял");
                    duelTimer = 0;
                    duelMember = null;
                }
                else
                    duelTimer++;
        }
    }
}