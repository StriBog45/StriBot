﻿using System.Collections.Concurrent;
using System.Collections.Generic;
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
    public class HalberdManager
    {
        private readonly ConcurrentDictionary<string, (Platform Platform, int Time)> _halberdDictionary;
        private readonly ReadyMadePhrases _readyMadePhrases;
        private readonly Currency _currency;
        private readonly Minute _minute;
        private readonly IDataBase _dataBase;

        private const int HalberdTimeoutTime = 5;

        public HalberdManager(ReadyMadePhrases readyMadePhrases, Currency currency, Minute minute, IDataBase dataBase)
        {
            _halberdDictionary = new ConcurrentDictionary<string, (Platform Platform, int Time)>();
            _readyMadePhrases = readyMadePhrases;
            _currency = currency;
            _minute = minute;
            _dataBase = dataBase;
        }

        public void Tick()
        {
            foreach (var user in _halberdDictionary)
            {
                var halberdValue = _halberdDictionary[user.Key];
                halberdValue.Time--;
                _halberdDictionary[user.Key] = halberdValue;

                if (_halberdDictionary[user.Key].Time <= 0)
                {
                    GlobalEventContainer.Message($"{user.Key} может использовать команды!", user.Value.Platform);
                    _halberdDictionary.TryRemove(user.Key, out _);
                }
            }
        }

        private Command CreateHalberdCommand()
        {
            var result = new Command("Алебарда", $"Запретить указанному пользователю использовать команды на {_minute.Incline(HalberdTimeoutTime)}. Цена: {PriceList.Halberd} {_currency.GenitiveMultiple}",
                delegate (CommandInfo commandInfo)
                {
                    if (commandInfo.ArgumentsAsList.Count == 1)
                    {
                        if (_dataBase.GetMoney(commandInfo.DisplayName) >= PriceList.Halberd)
                        {
                            _dataBase.AddMoney(commandInfo.DisplayName, -PriceList.Halberd);

                            if (_halberdDictionary.ContainsKey(commandInfo.ArgumentsAsList[0]))
                            {
                                var clearName = _dataBase.CleanNickname(commandInfo.ArgumentsAsList[0]);
                                var halberdValue = _halberdDictionary[clearName];
                                halberdValue.Time += HalberdTimeoutTime;
                                _halberdDictionary[clearName] = halberdValue;

                            }
                            else
                                _halberdDictionary.TryAdd(_dataBase.CleanNickname(commandInfo.ArgumentsAsList[0]), (commandInfo.Platform, HalberdTimeoutTime));

                            GlobalEventContainer.Message($"{commandInfo.DisplayName} использовал алебарду на {commandInfo.ArgumentsAsList[0]}! Цель обезаружена на {_minute.Incline(HalberdTimeoutTime)}!",
                                commandInfo.Platform);
                        }
                        else
                            _readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
                    }
                    else
                    {
                        _halberdDictionary.TryAdd(_dataBase.CleanNickname(commandInfo.DisplayName), (commandInfo.Platform, HalberdTimeoutTime));
                        GlobalEventContainer.Message($"{commandInfo.DisplayName} использовал алебарду на себя и не может использовать команды в течении {_minute.Incline(HalberdTimeoutTime)}!", 
                            commandInfo.Platform);
                    }
                }, new[] { "цель" }, CommandType.Interactive);

            return result;
        }

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>()
            {
                CreateHalberdCommand()
            };

        public bool CanSendMessage(CommandInfo commandInfo)
        {
            var clearName = _dataBase.CleanNickname(commandInfo.DisplayName);
            var result = !_halberdDictionary.ContainsKey(clearName);

            if (!result)
            {
                GlobalEventContainer.Message($"{commandInfo.DisplayName} не может использовать команды ещё {_minute.Incline(_halberdDictionary[clearName].Time)}!", commandInfo.Platform);
            }

            return result;
        }
    }
}