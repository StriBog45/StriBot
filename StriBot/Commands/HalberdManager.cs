using StriBot.Bots.Enums;
using StriBot.Commands.CommonFunctions;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using StriBot.Language;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace StriBot.Commands
{
    public class HalberdManager
    {
        private ConcurrentDictionary<string, (Platform Platform, int Time)> _halberdDictionary { get; set; }
        private ReadyMadePhrases _readyMadePhrases;
        private Currency _currency;
        private Minute _minute;

        private int halberdTime = 5;

        public HalberdManager(ReadyMadePhrases readyMadePhrases, Currency currency, Minute minute)
        {
            _halberdDictionary = new ConcurrentDictionary<string, (Platform Platform, int Time)>();
            _readyMadePhrases = readyMadePhrases;
            _currency = currency;
            _minute = minute; 
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

        public Command CreateHalberdCommand()
        {
            var result = new Command("Алебарда", $"Запретить указанному пользователю использовать команды на {_minute.Incline(halberdTime)}. Цена: {PriceList.Halberd} {_currency.GenitiveMultiple}",
                delegate (CommandInfo commandInfo)
                {
                    if (commandInfo.ArgumentsAsList.Count == 1)
                    {
                        if (DataBase.CheckMoney(commandInfo.DisplayName) >= PriceList.Halberd)
                        {
                            DataBase.AddMoneyToUser(commandInfo.DisplayName, -PriceList.Halberd);

                            if (_halberdDictionary.ContainsKey(commandInfo.ArgumentsAsList[0]))
                            {
                                var clearName = DataBase.CleanNickname(commandInfo.ArgumentsAsList[0]);
                                var halberdValue = _halberdDictionary[clearName];
                                halberdValue.Time += halberdTime;
                                _halberdDictionary[clearName] = halberdValue;

                            }
                            else
                                _halberdDictionary.TryAdd(DataBase.CleanNickname(commandInfo.ArgumentsAsList[0]), (commandInfo.Platform, halberdTime));

                            GlobalEventContainer.Message($"{commandInfo.DisplayName} использовал алебарду на {commandInfo.ArgumentsAsList[0]}! Цель обезаружена на {_minute.Incline(halberdTime)}!",
                                commandInfo.Platform);
                        }
                        else
                            _readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
                    }
                    else
                    {
                        _halberdDictionary.TryAdd(DataBase.CleanNickname(commandInfo.DisplayName), (commandInfo.Platform, halberdTime));
                        GlobalEventContainer.Message($"{commandInfo.DisplayName} использовал алебарду на себя и не может использовать команды в течении {_minute.Incline(halberdTime)}!", 
                            commandInfo.Platform);
                    }
                }, new string[] { "цель" }, CommandType.Interactive);

            return result;
        }

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>()
            {
                CreateHalberdCommand()
            };

        public bool CanSendMessage(CommandInfo commandInfo)
        {
            var clearName = DataBase.CleanNickname(commandInfo.DisplayName);
            var result = !_halberdDictionary.ContainsKey(clearName);

            if (!result)
            {
                GlobalEventContainer.Message($"{commandInfo.DisplayName} не может использовать команды ещё {_minute.Incline(_halberdDictionary[clearName].Time)}!", commandInfo.Platform);
            }

            return result;
        }
    }
}