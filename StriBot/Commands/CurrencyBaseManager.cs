using StriBot.Bots.Enums;
using StriBot.Commands.CommonFunctions;
using StriBot.Commands.Extensions;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using StriBot.Language;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StriBot.Commands
{
    public class CurrencyBaseManager
    {
        private readonly Currency _currency;
        private readonly ReadyMadePhrases _readyMadePhrases;
        private List<string> _receivedUsers;
        private int _distributionAmountUsers;
        private int _distributionAmountPerUsers;
        private int SubCoefficient { get => _subBonus ? subCoefficient : 1; }
        private int subCoefficient = 2;
        private bool _subBonus;
        public Dictionary<string, (int, int)> UsersBetted { get; set; }

        public CurrencyBaseManager(Currency currency, ReadyMadePhrases readyMadePhrases)
        {
            _currency = currency;
            _readyMadePhrases = readyMadePhrases;

            _receivedUsers = new List<string>();
        }

        public Command CreateStealCurrency()
        {
            Action<CommandInfo> action = delegate (CommandInfo commandInfo)
            {
                if (_distributionAmountUsers > 0)
                {
                    if (_receivedUsers.Where(x => x.CompareTo(commandInfo.DisplayName) == 0).ToArray().Count() == 0)
                    {
                        if (commandInfo.IsSubscriber.HasValue && commandInfo.IsSubscriber.Value)
                            DataBase.AddMoneyToUser(commandInfo.DisplayName, _distributionAmountPerUsers * SubCoefficient);
                        else
                            DataBase.AddMoneyToUser(commandInfo.DisplayName, _distributionAmountPerUsers);

                        GlobalEventContainer.Message($"{commandInfo.DisplayName} успешно стащил {_currency.Dative}!", commandInfo.Platform);
                        _distributionAmountUsers--;
                        _receivedUsers.Add(commandInfo.DisplayName);
                    }
                    else
                        GlobalEventContainer.Message($"{commandInfo.DisplayName} вы уже забрали {_currency.Dative}! Не жадничайте!", commandInfo.Platform);
                }
                else
                    GlobalEventContainer.Message($"{commandInfo.DisplayName} {_currency.GenitiveMultiple} не осталось!", commandInfo.Platform);
            };

            return new Command("Стащить", $"Крадет {_currency.Dative} без присмотра", action, CommandType.Interactive);
        }

        public void DistributionMoney(int perUser, int maxUsers, Platform platform, bool bonus = true)
        {
            _subBonus = bonus;
            _distributionAmountPerUsers = perUser;
            _distributionAmountUsers = maxUsers;
            GlobalEventContainer.Message($"Замечены {_currency.NominativeMultiple} без присмотра! Пиши !стащить striboF ", platform);
            _receivedUsers.Clear();
        }

        public Command CreateReturnCurrency()
        {
            Action<CommandInfo> action = delegate (CommandInfo commandInfo)
            {
                if (DataBase.CheckMoney(commandInfo.DisplayName) > 0)
                {
                    if (_distributionAmountPerUsers == 0)
                        _distributionAmountPerUsers = 1;

                    if (commandInfo.IsSubscriber.HasValue && commandInfo.IsSubscriber.HasValue)
                        DataBase.AddMoneyToUser(commandInfo.DisplayName, -_distributionAmountPerUsers * SubCoefficient);
                    else
                        DataBase.AddMoneyToUser(commandInfo.DisplayName, -_distributionAmountPerUsers);
                    GlobalEventContainer.Message($"{commandInfo.DisplayName} незаметно вернул {_currency.Dative}!", commandInfo.Platform);
                    _distributionAmountUsers++;
                    _receivedUsers.Remove(commandInfo.DisplayName);
                }
                else
                    _readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
            };

            return new Command("Вернуть", $"Возвращает {_currency.Dative} боту", action, CommandType.Interactive);
        }

        public Command CreateAddCurrency()
        {
            Action<CommandInfo> action = delegate (CommandInfo commandInfo)
            {
                if (commandInfo.ArgumentsAsList.Count == 2)
                {
                    DataBase.AddMoneyToUser(commandInfo.ArgumentsAsList[0], Convert.ToInt32(commandInfo.ArgumentsAsList[1]));
                    GlobalEventContainer.Message($"Вы успешно добавили {_currency.NominativeMultiple}! striboF", commandInfo.Platform);
                }
                else
                    _readyMadePhrases.IncorrectCommand(commandInfo.Platform);
            };

            return new Command("Добавить", $"Добавить объекту Х {_currency.GenitiveMultiple}. Только для владельца канала", Role.Broadcaster, action, new string[] { "объект", "количество" },
                CommandType.Interactive);
        }

        public Command CreateRemoveCurrency()
        {
            Action<CommandInfo> action = delegate (CommandInfo commandInfo) {
                if (commandInfo.ArgumentsAsList.Count == 2 && Convert.ToInt32(commandInfo.ArgumentsAsList[1]) > 0)
                {
                    DataBase.AddMoneyToUser(commandInfo.ArgumentsAsList[0], Convert.ToInt32(commandInfo.ArgumentsAsList[1]) * (-1));
                    GlobalEventContainer.Message($"Вы успешно изъяли {_currency.NominativeMultiple}! striboPeka ", commandInfo.Platform);
                }
                else
                    _readyMadePhrases.IncorrectCommand(commandInfo.Platform);
            };

            return new Command("Изъять", $"Изымает объект Х {_currency.GenitiveMultiple}", Role.Moderator, action, new string[] { "объект", "количество" }, CommandType.Interactive);
        }

        public Command CreateCheckBalance()
        {
            Action<CommandInfo> action = delegate (CommandInfo commandInfo) {
                if (commandInfo.ArgumentsAsList.Count == 0)
                {
                    var amount = DataBase.CheckMoney(commandInfo.DisplayName);
                    GlobalEventContainer.Message($"{commandInfo.DisplayName} имеет {_currency.Incline(amount, true)}! ", commandInfo.Platform);
                }
                else
                {
                    var amount = DataBase.CheckMoney(commandInfo.ArgumentsAsString);
                    GlobalEventContainer.Message($"{commandInfo.ArgumentsAsString} имеет {_currency.Incline(amount, true)}!", commandInfo.Platform);
                }
            };

            return new Command("Заначка", $"Текущие количество {_currency.GenitiveMultiple}", action, CommandType.Interactive);
        }

        public Command CreateTransferCurrency()
        {
            Action<CommandInfo> action = delegate (CommandInfo commandInfo) {
                if (commandInfo.ArgumentsAsList.Count == 2 && Int32.TryParse(commandInfo.ArgumentsAsList[1], out var amount) && amount > 0)
                {
                    if (DataBase.CheckMoney(commandInfo.DisplayName) >= amount)
                    {
                        DataBase.AddMoneyToUser(commandInfo.DisplayName, -amount);
                        DataBase.AddMoneyToUser(commandInfo.ArgumentsAsList[0], amount);
                        GlobalEventContainer.Message($"{commandInfo.DisplayName} подарил {_currency.Incline(amount, true)} {commandInfo.ArgumentsAsList[0]}! ", commandInfo.Platform);
                    }
                    else
                        _readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
                }
                else
                    _readyMadePhrases.IncorrectCommand(commandInfo.Platform);
            };

            return new Command("Подарить", $"Подарить {_currency.NominativeMultiple} [человек] [{_currency.GenitiveMultiple}] ", action, new string[] { "кому", "сколько" }, CommandType.Interactive);
        }

        public Command CreateDistributeCurrency()
        {
            Action<CommandInfo> action = delegate (CommandInfo commandInfo) {
                if (commandInfo.ArgumentsAsList.Count == 2
                    && Int32.TryParse(commandInfo.ArgumentsAsList[0], out int amountForPer)
                    && Int32.TryParse(commandInfo.ArgumentsAsList[1], out int amountPeople)
                    && amountForPer > 0
                    && amountPeople > 0)
                {
                    if (DataBase.CheckMoney(commandInfo.DisplayName) >= amountForPer * amountPeople)
                    {
                        DataBase.AddMoneyToUser(commandInfo.DisplayName, amountForPer * amountPeople * (-1));
                        DistributionMoney(amountForPer, amountPeople, commandInfo.Platform, false);
                    }
                    else
                        _readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
                }
                else
                    _readyMadePhrases.IncorrectCommand(commandInfo.Platform);

            };

            return new Command("Разбросать", $"Разбрасывает {_currency.NominativeMultiple} в чате, любой желающий может стащить", action,
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