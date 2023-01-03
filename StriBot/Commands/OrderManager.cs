using System;
using System.Collections.Generic;
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
    public class OrderManager
    {
        private readonly Currency _currency;
        private readonly ReadyMadePhrases _readyMadePhrases;
        private readonly IDataBase _dataBase;

        private readonly List<(string, string, int)> _listOrders;
        private Action<List<(string, string, int)>> _updateOrders;

        public OrderManager(Currency currency, ReadyMadePhrases readyMadePhrases, IDataBase dataBase)
        {
            _currency = currency;
            _readyMadePhrases = readyMadePhrases;
            _dataBase = dataBase;

            _listOrders = new List<(string, string, int)>();
        }

        public void SafeCallConnector(Action<List<(string, string, int)>> updateOrders)
            => _updateOrders = updateOrders;

        private Command CreateOrder()
            => new Command("Заказ", "Предложить свой заказ", CreateCustomOrderDelegate(), new[] { "Цена", "Описание" }, 
                CommandType.Order);

        private Command CreateOrderHero()
            => new Command("ЗаказГерой", $"Заказать героя на игру, цена: {_currency.Incline(PriceList.Hero)}", CreateOrderDelegate(PriceList.Hero), new[] { "Имя героя" }, 
                CommandType.Hidden);

        private Command CreateOrderCosplay()
            => new Command("ЗаказКосплей", $"Заказать косплей на трансляцию, цена: {_currency.Incline(PriceList.Cosplay)}", CreateOrderDelegate(PriceList.Cosplay), 
                new[] { "Имя героя" }, CommandType.Hidden);

        private Command CreateOrderGame()
            => new Command("ЗаказИгра", $"Заказать игру на 2 часа, цена: {_currency.Incline(PriceList.Game)}", CreateOrderDelegate(PriceList.Game), 
                new[] { "Название игры" }, CommandType.Hidden);

        private Command CreateOrderMovie()
            => new Command("ЗаказФильм", $"Просмотр фильма, цена: {_currency.Incline(PriceList.Movie)}", CreateOrderDelegate(PriceList.Movie),
                new[] { "Название фильма" }, CommandType.Hidden);

        private Command CreateOrderAnime()
            => new Command("ЗаказАниме", $"Просмотр серии аниме, цена: {_currency.Incline(PriceList.Anime)}", CreateOrderDelegate(PriceList.Anime), 
                new[] { "Название аниме" }, CommandType.Hidden);

        private Command CreateOrderVip()
            => new Command("ЗаказVIP", $"Покупка VIP, цена: {_currency.Incline(PriceList.Vip)}", CreateOrderDelegate(PriceList.Vip, "VIP"), CommandType.Hidden);

        private Command CreateOrderParty()
            => new Command("ЗаказГруппы", $"Совместная игру со стримером в Dota 2, цена: {_currency.Incline(PriceList.Group)}", CreateOrderDelegate(PriceList.Group, "группу"), 
                CommandType.Hidden);

        private Command CreateOrderBoost()
            => new Command("ЗаказБуст", $"Заказать стримера для подъема вашего рейтинга в Dota 2 (4 игры), цена: {_currency.Incline(PriceList.Boost)}", 
                CreateOrderDelegate(PriceList.Boost, "Буст"), CommandType.Hidden);

        private Command CreateOrderSong()
            => new Command("ЗаказПесня", $"Музыка на стриме! Цена: {_currency.Incline(PriceList.Song)}", CreateOrderDelegate(PriceList.Song),
                new[] { "Ссылка на песню" }, CommandType.Hidden);

        private Command CreateWorkout()
            => new Command("Разминка", $"Стример разомнись! Цена: {_currency.Incline(PriceList.Workout)}", CreateOrderDelegate(PriceList.Workout), CommandType.Hidden);

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>
            {
                CreateOrder(),
                //CreateOrderHero(),
                //CreateOrderCosplay(),
                //CreateOrderGame(),
                //CreateOrderMovie(),
                //CreateOrderAnime(),
                //CreateOrderVip(),
                //CreateOrderParty(),
                //CreateOrderBoost(),
                //CreateOrderSong(),
                //CreateWorkout()
            };

        public void OrderRemove(string orderName, string customer, int price)
            => _listOrders.Remove((orderName, customer, price));

        private Action<CommandInfo> CreateOrderDelegate(int price, string product)
        {
            return delegate (CommandInfo commandInfo)
            {
                if (_dataBase.GetMoney(commandInfo.DisplayName) >= price)
                {
                    _listOrders.Add((product, commandInfo.DisplayName, price));
                    GlobalEventContainer.Message($"{commandInfo.DisplayName} успешно сделал заказ! {_currency.NominativeMultiple.Title()} будут сняты после принятия заказа", commandInfo.Platform);
                    _updateOrders(_listOrders);
                }
                else
                    _readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
            };
        }

        private Action<CommandInfo> CreateOrderDelegate(int price)
        {
            return delegate (CommandInfo commandInfo)
            {
                if (_dataBase.GetMoney(commandInfo.DisplayName) >= price)
                {
                    if (commandInfo.ArgumentsAsList.Count != 0)
                    {
                        _listOrders.Add((commandInfo.ArgumentsAsString, commandInfo.DisplayName, price));
                        GlobalEventContainer.Message($"{commandInfo.DisplayName} успешно сделал заказ!", commandInfo.Platform);
                        _updateOrders(_listOrders);
                    }
                    else
                        ReadyMadePhrases.IncorrectCommand(commandInfo.Platform);
                }
                else
                    _readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
            };
        }

        private Action<CommandInfo> CreateCustomOrderDelegate()
            => delegate (CommandInfo commandInfo)
            {
                if (commandInfo.ArgumentsAsList.Count > 1)
                {
                    if (int.TryParse(commandInfo.ArgumentsAsList[0], out var customPrice))
                    {
                        CreateOrderDelegate(customPrice);
                        if (_dataBase.GetMoney(commandInfo.DisplayName) >= customPrice)
                        {
                            _listOrders.Add((commandInfo.ArgumentsAsString.Substring(commandInfo.ArgumentsAsList[0].Length + 1), commandInfo.DisplayName, customPrice));
                            GlobalEventContainer.Message($"{commandInfo.DisplayName} успешно сделал заказ!", commandInfo.Platform);
                            _updateOrders(_listOrders);
                        }
                        else
                            _readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
                    }
                    else
                        ReadyMadePhrases.IncorrectCommand(commandInfo.Platform);
                }
            };
    }
}