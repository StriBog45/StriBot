using System;
using System.Collections.Generic;
using System.Linq;
using StriBot.Bots.Enums;
using StriBot.Commands.CommonFunctions;
using StriBot.Commands.Enums;
using StriBot.Commands.Extensions;
using StriBot.Commands.Models;
using StriBot.DateBase.Interfaces;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using StriBot.Language.Extensions;
using StriBot.Language.Implementations;

namespace StriBot.Commands
{
    public class CurrencyBaseManager
    {
        private readonly Currency _currency;
        private readonly ReadyMadePhrases _readyMadePhrases;
        private readonly IDataBase _dataBase;
        private readonly List<string> _receivedUsers;
        private readonly Dictionary<string, DateTime> _userLastMessage;
        private int _distributionAmountUsers;
        private int _distributionAmountPerUsers;
        private int SubCoefficient => _subBonus ? subCoefficient : 1;
        private int subCoefficient = 2;
        private bool _subBonus;

        public CurrencyBaseManager(Currency currency, ReadyMadePhrases readyMadePhrases, IDataBase dataBase)
        {
            _currency = currency;
            _readyMadePhrases = readyMadePhrases;
            _dataBase = dataBase;

            _receivedUsers = new List<string>();
            _userLastMessage = new Dictionary<string, DateTime>();
        }

        private Command CreateStealCurrency()
        {
            void Action(CommandInfo commandInfo)
            {
                if (_distributionAmountUsers > 0)
                {
                    if (!_receivedUsers.Where(x => x.CompareTo(commandInfo.DisplayName) == 0).ToArray().Any())
                    {
                        if (commandInfo.IsSubscriber.HasValue && commandInfo.IsSubscriber.Value)
                            _dataBase.AddMoney(commandInfo.DisplayName, _distributionAmountPerUsers * SubCoefficient);
                        else
                            _dataBase.AddMoney(commandInfo.DisplayName, _distributionAmountPerUsers);

                        GlobalEventContainer.Message($"{commandInfo.DisplayName} успешно стащил {_currency.Accusative}!", commandInfo.Platform);
                        _distributionAmountUsers--;
                        _receivedUsers.Add(commandInfo.DisplayName);
                    }
                    else
                        GlobalEventContainer.Message($"{commandInfo.DisplayName} вы уже забрали {_currency.Accusative}! Не жадничайте!", commandInfo.Platform);
                }
                else
                    GlobalEventContainer.Message($"{commandInfo.DisplayName} {_currency.GenitiveMultiple} не осталось!", commandInfo.Platform);
            }

            return new Command("Стащить", $"Крадет {_currency.Accusative} без присмотра", Action, CommandType.Interactive);
        }

        public void DistributionMoney(int perUser, int maxUsers, Platform platform, bool bonus = true)
        {
            _subBonus = bonus;
            _distributionAmountPerUsers = perUser;
            _distributionAmountUsers = maxUsers;
            GlobalEventContainer.Message($"Замечены {_currency.NominativeMultiple} без присмотра! PogChamp ", platform);
            _receivedUsers.Clear();
        }

        private Command CreateReturnCurrency()
        {
            void Action(CommandInfo commandInfo)
            {
                if (_dataBase.GetMoney(commandInfo.DisplayName) > 0)
                {
                    if (_distributionAmountPerUsers == 0) _distributionAmountPerUsers = 1;

                    if (commandInfo.IsSubscriber.HasValue && commandInfo.IsSubscriber.HasValue)
                        _dataBase.AddMoney(commandInfo.DisplayName, -_distributionAmountPerUsers * SubCoefficient);
                    else
                        _dataBase.AddMoney(commandInfo.DisplayName, -_distributionAmountPerUsers);
                    GlobalEventContainer.Message($"{commandInfo.DisplayName} незаметно вернул {_currency.Accusative}!", commandInfo.Platform);
                    _distributionAmountUsers++;
                    _receivedUsers.Remove(commandInfo.DisplayName);
                }
                else
                    _readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
            }

            return new Command("Вернуть", $"Возвращает {_currency.Accusative} боту", Action, CommandType.Interactive);
        }

        private Command CreateAddCurrency()
        {
            void Action(CommandInfo commandInfo)
            {
                if (commandInfo.ArgumentsAsList.Count == 2)
                {
                    _dataBase.AddMoney(commandInfo.ArgumentsAsList[0], Convert.ToInt32(commandInfo.ArgumentsAsList[1]));
                    GlobalEventContainer.Message($"Вы успешно добавили {_currency.NominativeMultiple}! striboF", commandInfo.Platform);
                }
                else
                    ReadyMadePhrases.IncorrectCommand(commandInfo.Platform);
            }

            return new Command("Добавить", $"Добавить объекту Х {_currency.GenitiveMultiple}. Только для владельца канала", Role.Broadcaster, Action, new[] { "объект", "количество" },
                CommandType.Interactive);
        }

        private Command CreateRemoveCurrency()
        {
            void Action(CommandInfo commandInfo)
            {
                if (commandInfo.ArgumentsAsList.Count == 2 && Convert.ToInt32(commandInfo.ArgumentsAsList[1]) > 0)
                {
                    _dataBase.AddMoney(commandInfo.ArgumentsAsList[0], Convert.ToInt32(commandInfo.ArgumentsAsList[1]) * (-1));
                    GlobalEventContainer.Message($"Успешно изъяли {_currency.NominativeMultiple}! striboPeka ", commandInfo.Platform);
                }
                else
                    ReadyMadePhrases.IncorrectCommand(commandInfo.Platform);
            }

            return new Command("Изъять", $"Изымает объект Х {_currency.GenitiveMultiple}", Role.Moderator, Action, new[] { "объект", "количество" }, CommandType.Interactive);
        }

        private Command CreateCheckBalance()
        {
            void Action(CommandInfo commandInfo)
            {
                if (commandInfo.ArgumentsAsList.Count == 0)
                {
                    var amount = _dataBase.GetMoney(commandInfo.DisplayName);
                    GlobalEventContainer.Message($"{commandInfo.DisplayName} имеет {_currency.Incline(amount, true)}! ", commandInfo.Platform);
                }
                else
                {
                    var amount = _dataBase.GetMoney(commandInfo.ArgumentsAsString);
                    GlobalEventContainer.Message($"{commandInfo.ArgumentsAsString} имеет {_currency.Incline(amount, true)}!", commandInfo.Platform);
                }
            }

            return new Command("Заначка", $"Текущие количество {_currency.GenitiveMultiple}", Action, CommandType.Interactive);
        }

        private Command CreateTransferCurrency()
        {
            void Action(CommandInfo commandInfo)
            {
                if (commandInfo.ArgumentsAsList.Count == 2 && int.TryParse(commandInfo.ArgumentsAsList[1], out var amount) && amount > 0)
                {
                    if (_dataBase.GetMoney(commandInfo.DisplayName) >= amount)
                    {
                        _dataBase.AddMoney(commandInfo.DisplayName, -amount);
                        _dataBase.AddMoney(commandInfo.ArgumentsAsList[0], amount);
                        GlobalEventContainer.Message($"{commandInfo.DisplayName} подарил {_currency.Incline(amount, true)} {commandInfo.ArgumentsAsList[0]}! ", commandInfo.Platform);
                    }
                    else
                        _readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
                }
                else
                    ReadyMadePhrases.IncorrectCommand(commandInfo.Platform);
            }

            return new Command("Подарить", $"Подарить {_currency.NominativeMultiple} [человек] [{_currency.GenitiveMultiple}] ", Action, new[] { "кому", "сколько" }, CommandType.Interactive);
        }

        private Command CreateDistributeCurrency()
        {
            void Action(CommandInfo commandInfo)
            {
                if (commandInfo.ArgumentsAsList.Count == 2 && int.TryParse(commandInfo.ArgumentsAsList[0], out var amountForPer) && int.TryParse(commandInfo.ArgumentsAsList[1], out int amountPeople) && amountForPer > 0 && amountPeople > 0)
                {
                    if (_dataBase.GetMoney(commandInfo.DisplayName) >= amountForPer * amountPeople)
                    {
                        _dataBase.AddMoney(commandInfo.DisplayName, amountForPer * amountPeople * (-1));
                        DistributionMoney(amountForPer, amountPeople, commandInfo.Platform, false);
                    }
                    else
                        _readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
                }
                else
                    ReadyMadePhrases.IncorrectCommand(commandInfo.Platform);
            }

            return new Command("Разбросать", $"Разбрасывает {_currency.NominativeMultiple} в чате, любой желающий может стащить", Action,
                new[] { "Сколько стащит за раз", "Сколько человек сможет стащить" }, CommandType.Interactive);
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

        public void ReceivedMessage(string displayName)
        {
            var cleanName = _dataBase.CleanNickname(displayName);
            
            if (!_userLastMessage.ContainsKey(cleanName))
            {
                _userLastMessage.Add(cleanName, DateTime.Now);
                _dataBase.AddMoney(cleanName, 1);
            }
            else if (_userLastMessage[cleanName] < DateTime.Now.AddHours(-1))
            {
                _userLastMessage[cleanName] = DateTime.Now;
                _dataBase.AddMoney(cleanName, 1);
            }
        }
    }
}