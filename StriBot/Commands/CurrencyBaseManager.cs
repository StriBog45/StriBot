using StriBot.Commands.CommonFunctions;
using StriBot.Language;
using StriBot.TwitchBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Events;

namespace StriBot.Commands
{
    public class CurrencyBaseManager
    {
        private readonly ITwitchBot twitchBot;
        private readonly Currency currency;
        private readonly ReadyMadePhrases readyMadePhrases;
        private List<string> receivedUsers;
        private int distributionAmountUsers { get; set; }
        private int distributionAmountPerUsers { get; set; }
        private int subCoefficient = 2;
        private int SubCoefficient { get => subBonus ? subCoefficient : 1; }
        private bool subBonus;
        public Dictionary<string, (int, int)> UsersBetted { get; set; }

        public CurrencyBaseManager(ITwitchBot twitchBot, Currency currency, ReadyMadePhrases readyMadePhrases)
        {
            this.twitchBot = twitchBot;
            this.currency = currency;
            this.readyMadePhrases = readyMadePhrases;

            receivedUsers = new List<string>();
        }

        public Command CreateStealCurrency()
        {
            Action<OnChatCommandReceivedArgs> action = delegate (OnChatCommandReceivedArgs e)
            {
                if (distributionAmountUsers > 0)
                {
                    if (receivedUsers.Where(x => x.CompareTo(e.Command.ChatMessage.DisplayName) == 0).ToArray().Count() == 0)
                    {
                        if (e.Command.ChatMessage.IsSubscriber)
                            DataBase.AddMoneyToUser(e.Command.ChatMessage.DisplayName, distributionAmountPerUsers * SubCoefficient);
                        else
                            DataBase.AddMoneyToUser(e.Command.ChatMessage.DisplayName, distributionAmountPerUsers);

                        twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} успешно стащил {currency.Dative}!");
                        distributionAmountUsers--;
                        receivedUsers.Add(e.Command.ChatMessage.DisplayName);
                    }
                    else
                        twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} вы уже забрали {currency.Dative}! Не жадничайте!");
                }
                else
                    twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} {currency.GenitiveMultiple} не осталось!");
            };

            return new Command("Стащить", $"Крадет {currency.Dative} без присмотра", action, CommandType.Interactive);
        }

        public void DistributionMoney(int perUser, int maxUsers, bool bonus = true)
        {
            subBonus = bonus;
            distributionAmountPerUsers = perUser;
            distributionAmountUsers = maxUsers;
            twitchBot.SendMessage($"Замечены {currency.NominativeMultiple} без присмотра! Время полоскать! Пиши !стащить striboF ");
            receivedUsers.Clear();
        }

        public Command CreateReturnCurrency()
        {
            Action<OnChatCommandReceivedArgs> action = delegate (OnChatCommandReceivedArgs e)
            {
                if (DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) > 0)
                {
                    if (distributionAmountPerUsers == 0)
                        distributionAmountPerUsers = 1;

                    if (e.Command.ChatMessage.IsSubscriber)
                        DataBase.AddMoneyToUser(e.Command.ChatMessage.DisplayName, -distributionAmountPerUsers * SubCoefficient);
                    else
                        DataBase.AddMoneyToUser(e.Command.ChatMessage.DisplayName, -distributionAmountPerUsers);
                    twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} незаметно вернул {currency.Dative}!");
                    distributionAmountUsers++;
                    receivedUsers.Remove(e.Command.ChatMessage.DisplayName);
                }
                else
                    readyMadePhrases.NoMoney(e.Command.ChatMessage.DisplayName);
            };

            return new Command("Вернуть", $"Возвращает {currency.Dative} боту", action, CommandType.Interactive);
        }

        public Command CreateAddCurrency()
        {
            Action<OnChatCommandReceivedArgs> action = delegate (OnChatCommandReceivedArgs e)
            {
                if (e.Command.ArgumentsAsList.Count == 2)
                {
                    DataBase.AddMoneyToUser(e.Command.ArgumentsAsList[0], Convert.ToInt32(e.Command.ArgumentsAsList[1]));
                    twitchBot.SendMessage($"Вы успешно добавили {currency.NominativeMultiple}! striboF");
                }
                else
                    readyMadePhrases.IncorrectCommand();
            };

            return new Command("Добавить", $"Добавить объекту Х {currency.GenitiveMultiple}. Только для владельца канала", Role.Broadcaster, action, new string[] { "объект", "количество" },
                CommandType.Interactive);
        }

        public Command CreateRemoveCurrency()
        {
            Action<OnChatCommandReceivedArgs> action = delegate (OnChatCommandReceivedArgs e) {
                if (e.Command.ArgumentsAsList.Count == 2 && Convert.ToInt32(e.Command.ArgumentsAsList[1]) > 0)
                {
                    DataBase.AddMoneyToUser(e.Command.ArgumentsAsList[0], Convert.ToInt32(e.Command.ArgumentsAsList[1]) * (-1));
                    twitchBot.SendMessage($"Вы успешно изъяли {currency.NominativeMultiple}! striboPeka ");
                }
                else
                    readyMadePhrases.IncorrectCommand();
            };

            return new Command("Изъять", $"Изымает объект Х {currency.GenitiveMultiple}", Role.Moderator, action, new string[] { "объект", "количество" }, CommandType.Interactive);
        }

        public Command CreateCheckBalance()
        {
            Action<OnChatCommandReceivedArgs> action = delegate (OnChatCommandReceivedArgs e) {
                if (e.Command.ArgumentsAsList.Count == 0)
                {
                    var amount = DataBase.CheckMoney(e.Command.ChatMessage.DisplayName);
                    twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} имеет {amount} {currency.Incline(amount, true)}! ");
                }
                else
                {
                    var amount = DataBase.CheckMoney(e.Command.ArgumentsAsString);
                    twitchBot.SendMessage($"{e.Command.ArgumentsAsString} имеет {amount} {currency.Incline(amount, true)}!");
                }
            };

            return new Command("Заначка", $"Текущие количество {currency.GenitiveMultiple} у вас", action, CommandType.Interactive);
        }

        public Command CreateTransferCurrency()
        {
            Action<OnChatCommandReceivedArgs> action = delegate (OnChatCommandReceivedArgs e) {
                if (e.Command.ArgumentsAsList.Count == 2 && Int32.TryParse(e.Command.ArgumentsAsList[1], out var amount) && amount > 0)
                {
                    if (DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) >= amount)
                    {
                        DataBase.AddMoneyToUser(e.Command.ChatMessage.DisplayName, -amount);
                        DataBase.AddMoneyToUser(e.Command.ArgumentsAsList[0], amount);
                        twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} подарил {amount} {currency.Incline(amount, true)} {e.Command.ArgumentsAsList[0]}! ");
                    }
                    else
                        readyMadePhrases.NoMoney(e.Command.ChatMessage.DisplayName);
                }
                else
                    readyMadePhrases.IncorrectCommand();
            };

            return new Command("Подарить", $"Подарить {currency.NominativeMultiple} [человек] [{currency.GenitiveMultiple}] ", action, new string[] { "кому", "сколько" }, CommandType.Interactive);
        }

        public Command CreateDistributeCurrency()
        {
            Action<OnChatCommandReceivedArgs> action = delegate (OnChatCommandReceivedArgs e) {
                if (e.Command.ArgumentsAsList.Count == 2
                    && Int32.TryParse(e.Command.ArgumentsAsList[0], out int amountForPer)
                    && Int32.TryParse(e.Command.ArgumentsAsList[1], out int amountPeople)
                    && amountForPer > 0
                    && amountPeople > 0)
                {
                    if (DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) >= amountForPer * amountPeople)
                    {
                        DataBase.AddMoneyToUser(e.Command.ChatMessage.DisplayName, amountForPer * amountPeople * (-1));
                        DistributionMoney(amountForPer, amountPeople, false);
                    }
                    else
                        readyMadePhrases.NoMoney(e.Command.ChatMessage.DisplayName);
                }
                else
                    readyMadePhrases.IncorrectCommand();

            };

            return new Command("Разбросать", $"Разбрасывает {currency.NominativeMultiple} в чате, любой желающий может стащить", action,
                new string[] { "Сколько стащит за раз", "Сколько человек сможет стащить" }, CommandType.Interactive);
        }

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>()
            {
                CreateStealCurrency(),
                CreateReturnCurrency(),
                CreateAddCurrency(),
                CreateTransferCurrency(),
                CreateRemoveCurrency(),
                CreateCheckBalance(),
                CreateDistributeCurrency()
            };
    }
}