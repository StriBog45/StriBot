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
    public class OrderManager
    {
        private readonly Currency _currency;
        private readonly ReadyMadePhrases _readyMadePhrases;

        private readonly List<(string, string, int)> _listOrders;
        private Action<List<(string, string, int)>> _updateOrders;

        public OrderManager(Currency currency, ReadyMadePhrases readyMadePhrases)
        {
            _currency = currency;
            _readyMadePhrases = readyMadePhrases;

            _listOrders = new List<(string, string, int)>();
        }

        public void SafeCallConnector(Action<List<(string, string, int)>> updateOrders)
            => _updateOrders = updateOrders;

        private Command CreateOrder()
            => new Command("Заказ", "Предложить свой заказ", CreateCustomOrderDelegate(), new[] { "Цена", "Описание" }, 
                CommandType.Order);

        private Command CreateOrderHero()
            => new Command("ЗаказГерой", $"Заказать героя на игру, цена: {_currency.Incline(PriceList.Hero)}", CreateOrderDelegate(PriceList.Hero), new[] { "Имя героя" }, 
                CommandType.Order);

        private Command CreateOrderCosplay()
            => new Command("ЗаказКосплей", $"Заказать косплей на трансляцию, цена: {_currency.Incline(PriceList.Cosplay)}", CreateOrderDelegate(PriceList.Cosplay), 
                new[] { "Имя героя" }, CommandType.Hidden);

        private Command CreateOrderGame()
            => new Command("ЗаказИгра", $"Заказать игру на трансляцию, цена: {_currency.Incline(PriceList.Game)}", CreateOrderDelegate(PriceList.Game), 
                new[] { "Название игры" }, CommandType.Order);

        private Command CreateOrderMovie()
            => new Command("ЗаказФильм", $"Заказать фильм на трансляцию, цена: {_currency.Incline(PriceList.Movie)}", CreateOrderDelegate(PriceList.Movie),
                new[] { "Название фильма" }, CommandType.Order);

        private Command CreateOrderAnime()
            => new Command("ЗаказАниме", $"Заказать серию аниме на трансляцию, цена: {_currency.Incline(PriceList.Anime)}", CreateOrderDelegate(PriceList.Anime), 
                new[] { "Название аниме" }, CommandType.Order);

        private Command CreateOrderVip()
            => new Command("ЗаказVIP", $"Купить VIP, цена: {_currency.Incline(PriceList.Vip)}", CreateOrderDelegate(PriceList.Vip, "VIP"), CommandType.Order);

        private Command CreateOrderParty()
            => new Command("ЗаказГруппы", $"Заказать совместную игру со стримером в Dota 2, цена: {_currency.Incline(PriceList.Group)}", CreateOrderDelegate(PriceList.Group, "группу"), 
                CommandType.Order);

        private Command CreateOrderBoost()
            => new Command("ЗаказБуст", $"Заказать стримера для подъема вашего рейтинга в Dota 2 на 1 трансляцию (4 игры), цена: {_currency.Incline(PriceList.Boost)}", 
                CreateOrderDelegate(PriceList.Boost, "Буст"), CommandType.Order);

        private Command CreateOrderSong()
            => new Command("ЗаказПесня", $"Заказать воспроизведение песни, цена: {_currency.Incline(PriceList.Song)}", CreateOrderDelegate(PriceList.Song),
                new[] { "Ссылка на песню" }, CommandType.Order);

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>
            {
                CreateOrder(),
                CreateOrderHero(),
                CreateOrderCosplay(),
                CreateOrderGame(),
                CreateOrderMovie(),
                CreateOrderAnime(),
                CreateOrderVip(),
                CreateOrderParty(),
                CreateOrderBoost(),
                CreateOrderSong()
            };

        public void OrderRemove(string orderName, string customer, int price)
            => _listOrders.Remove((orderName, customer, price));

        private Action<CommandInfo> CreateOrderDelegate(int price, string product)
        {
            return delegate (CommandInfo commandInfo)
            {
                if (DataBase.GetMoney(commandInfo.DisplayName) >= price)
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
                if (DataBase.GetMoney(commandInfo.DisplayName) >= price)
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
                        if (DataBase.GetMoney(commandInfo.DisplayName) >= customPrice)
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