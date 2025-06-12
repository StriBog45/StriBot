using System;
using System.Collections.Generic;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Commands.Extensions;
using StriBot.Application.Commands.Models;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;
using StriBot.Application.Localization;
using StriBot.Application.Localization.Implementations;

namespace StriBot.Application.Commands.Handlers;

public class OrderHandler(Currency currency, ReadyMadePhrases readyMadePhrases, IDataBase dataBase)
{
    private readonly List<(string, string, int)> _listOrders = [];
    private Action<List<(string, string, int)>> _updateOrders;

    public void SafeCallConnector(Action<List<(string, string, int)>> updateOrders)
        => _updateOrders = updateOrders;

    private Command CreateOrder()
        => new Command("Заказ", "Предложить свой заказ", CreateCustomOrderDelegate(), new[] { "Цена", "Описание" }, 
            CommandType.Order);

    // private Command CreateOrderHero()
    //     => new Command("ЗаказГерой", $"Заказать героя на игру, цена: {currency.Incline(PriceList.Hero)}", CreateOrderDelegate(PriceList.Hero), new[] { "Имя героя" }, 
    //         CommandType.Hidden);
    //
    // private Command CreateOrderCosplay()
    //     => new Command("ЗаказКосплей", $"Заказать косплей на трансляцию, цена: {currency.Incline(PriceList.Cosplay)}", CreateOrderDelegate(PriceList.Cosplay),
    //         ["Имя героя"], CommandType.Hidden);
    //
    // private Command CreateOrderGame()
    //     => new Command("ЗаказИгра", $"Заказать игру на 2 часа, цена: {currency.Incline(PriceList.Game)}", CreateOrderDelegate(PriceList.Game), 
    //         new[] { "Название игры" }, CommandType.Hidden);
    //
    // private Command CreateOrderMovie()
    //     => new Command("ЗаказФильм", $"Просмотр фильма, цена: {currency.Incline(PriceList.Movie)}", CreateOrderDelegate(PriceList.Movie),
    //         new[] { "Название фильма" }, CommandType.Hidden);
    //
    // private Command CreateOrderAnime()
    //     => new Command("ЗаказАниме", $"Просмотр серии аниме, цена: {currency.Incline(PriceList.Anime)}", CreateOrderDelegate(PriceList.Anime), 
    //         new[] { "Название аниме" }, CommandType.Hidden);
    //
    // private Command CreateOrderVip()
    //     => new Command("ЗаказVIP", $"Покупка VIP, цена: {currency.Incline(PriceList.Vip)}", CreateOrderDelegate(PriceList.Vip, "VIP"), CommandType.Hidden);
    //
    // private Command CreateOrderParty()
    //     => new Command("ЗаказГруппы", $"Совместная игру со стримером в Dota 2, цена: {currency.Incline(PriceList.Group)}", CreateOrderDelegate(PriceList.Group, "группу"), 
    //         CommandType.Hidden);
    //
    // private Command CreateOrderBoost()
    //     => new Command("ЗаказБуст", $"Заказать стримера для подъема вашего рейтинга в Dota 2 (4 игры), цена: {currency.Incline(PriceList.Boost)}", 
    //         CreateOrderDelegate(PriceList.Boost, "Буст"), CommandType.Hidden);
    //
    // private Command CreateOrderSong()
    //     => new Command("ЗаказПесня", $"Музыка на стриме! Цена: {currency.Incline(PriceList.Song)}", CreateOrderDelegate(PriceList.Song),
    //         new[] { "Ссылка на песню" }, CommandType.Hidden);
    //
    // private Command CreateWorkout()
    //     => new Command("Разминка", $"Стример разомнись! Цена: {currency.Incline(PriceList.Workout)}", CreateOrderDelegate(PriceList.Workout), CommandType.Hidden);

    public Dictionary<string, Command> CreateCommands()
        => new()
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

    // private Action<CommandInfo> CreateOrderDelegate(int price, string product)
    // {
    //     return delegate (CommandInfo commandInfo)
    //     {
    //         if (dataBase.GetMoney(commandInfo.DisplayName) >= price)
    //         {
    //             _listOrders.Add((product, commandInfo.DisplayName, price));
    //             EventContainer.Message($"{commandInfo.DisplayName} успешно сделал заказ! {currency.NominativeMultiple.Title()} будут сняты после принятия заказа", commandInfo.Platform);
    //             _updateOrders(_listOrders);
    //         }
    //         else
    //             readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
    //     };
    // }
    //
    private Action<CommandInfo> CreateOrderDelegate(int price)
    {
        return delegate (CommandInfo commandInfo)
        {
            if (dataBase.GetMoney(commandInfo.DisplayName) >= price)
            {
                if (commandInfo.ArgumentsAsList.Count != 0)
                {
                    _listOrders.Add((commandInfo.ArgumentsAsString, commandInfo.DisplayName, price));
                    EventContainer.Message($"{commandInfo.DisplayName} успешно сделал заказ!", commandInfo.Platform);
                    _updateOrders(_listOrders);
                }
                else
                    ReadyMadePhrases.IncorrectCommand(commandInfo.Platform);
            }
            else
                readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
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
                    if (dataBase.GetMoney(commandInfo.DisplayName) >= customPrice)
                    {
                        _listOrders.Add((commandInfo.ArgumentsAsString.Substring(commandInfo.ArgumentsAsList[0].Length + 1), commandInfo.DisplayName, customPrice));
                        EventContainer.Message($"{commandInfo.DisplayName} успешно сделал заказ!", commandInfo.Platform);
                        _updateOrders(_listOrders);
                    }
                    else
                        readyMadePhrases.NoMoney(commandInfo.DisplayName, commandInfo.Platform);
                }
                else
                    ReadyMadePhrases.IncorrectCommand(commandInfo.Platform);
            }
        };
}