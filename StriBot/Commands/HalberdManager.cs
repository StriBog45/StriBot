using StriBot.Commands.CommonFunctions;
using StriBot.Language;
using StriBot.TwitchBot.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TwitchLib.Client.Events;

namespace StriBot.Commands
{
    public class HalberdManager
    {
        private ConcurrentDictionary<string, int> halberdDictionary { get; set; }
        private ReadyMadePhrases readyMadePhrases;
        private ITwitchBot twitchBot;
        private Currency currency;
        private Minute minute;

        private int halberdTime = 5;

        public HalberdManager(ReadyMadePhrases readyMadePhrases, ITwitchBot twitchBot, Currency currency, Minute minute)
        {
            halberdDictionary = new ConcurrentDictionary<string, int>();
            this.readyMadePhrases = readyMadePhrases;
            this.twitchBot = twitchBot;
            this.currency = currency;
            this.minute = minute; 
        }

        public void Tick()
        {
            foreach (var user in halberdDictionary)
            {
                halberdDictionary[user.Key]--;
                if (halberdDictionary[user.Key] <= 0)
                {
                    twitchBot.SendMessage($"{user.Key} может использовать команды!");
                    halberdDictionary.TryRemove(user.Key, out _);
                }
            }
        }

        public Command CreateHalberdCommand()
        {
            var result = new Command("Алебарда", $"Запретить указанному пользователю использовать команды на {halberdTime} {minute.Incline(halberdTime)}. Цена: {PriceList.Halberd} {currency.GenitiveMultiple}",
                delegate (OnChatCommandReceivedArgs e)
                {
                    if (e.Command.ArgumentsAsList.Count == 1)
                    {
                        if (DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) >= PriceList.Halberd)
                        {
                            DataBase.AddMoneyToUser(e.Command.ChatMessage.DisplayName, -PriceList.Halberd);

                            if (halberdDictionary.ContainsKey(e.Command.ArgumentsAsList[0]))
                                halberdDictionary[DataBase.CleanNickname(e.Command.ArgumentsAsList[0])] += halberdTime;
                            else
                                halberdDictionary.TryAdd(DataBase.CleanNickname(e.Command.ArgumentsAsList[0]), halberdTime);

                            twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} использовал алебарду на {e.Command.ArgumentsAsList[0]}! Цель обезаружена на {halberdTime} {minute.Incline(halberdTime)}!");
                        }
                        else
                            readyMadePhrases.NoMoney(e.Command.ChatMessage.DisplayName);
                    }
                    else
                    {
                        halberdDictionary.TryAdd(DataBase.CleanNickname(e.Command.ChatMessage.DisplayName), halberdTime);
                        twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} использовал алебарду на себя и не может использовать команды в течении {halberdTime} {minute.Incline(halberdTime)}!");
                    }
                }, new string[] { "цель" }, CommandType.Interactive);

            return result;
        }

        public Dictionary<string, Command> CreateCommands()
        {
            var result = new Dictionary<string, Command>()
            {
                CreateHalberdCommand()
            };
            return result;
        }

        public bool CanSendMessage(string name)
        {
            var clearName = DataBase.CleanNickname(name);
            var result = !halberdDictionary.ContainsKey(clearName);

            if (!result)
            {
                twitchBot.SendMessage($"{name} не может использовать команды ещё {halberdDictionary[clearName]} {minute.Incline(halberdDictionary[clearName])}!");
            }

            return result;
        }
    }
}