using StriBot.Commands.CommonFunctions;
using StriBot.Language;
using StriBot.TwitchBot.Interfaces;
using System;
using System.Collections.Generic;
using TwitchLib.Client.Events;

namespace StriBot.Commands
{
    public class OrderManager
    {
        private readonly ITwitchBot twitchBot;
        private readonly Currency currency;
        private readonly ReadyMadePhrases readyMadePhrases;

        private List<(string, string, int)> listOrders;
        private Action<List<(string, string, int)>> updateOrders;

        public OrderManager(ITwitchBot twitchBot, Currency currency, ReadyMadePhrases readyMadePhrases)
        {
            this.twitchBot = twitchBot;
            this.currency = currency;
            this.readyMadePhrases = readyMadePhrases;

            listOrders = new List<(string, string, int)>();
        }

        public void SafeCallConnector(Action<List<(string, string, int)>> updateOrders)
        {
            this.updateOrders = updateOrders;
        }

        public Command CreateOrder()
            => new Command("Заказ", String.Format("Предложить свой заказ", PriceList.Hero), CreateCustomOrderDelegate(), new string[] { currency.NominativeMultiple.Title(), "Заказ" }, 
                CommandType.Order);

        public Command CreateOrderHero()
            => new Command("ЗаказГерой", $"Заказать героя на игру, цена: {PriceList.Hero} {currency.Incline(PriceList.Hero)}", CreateOrderDelegate(PriceList.Hero), new string[] { "Имя героя" }, 
                CommandType.Order);

        public Command CreateOrderCosplay()
            => new Command("ЗаказКосплей", $"Заказать косплей на трансляцию, цена: {PriceList.Cosplay} {currency.Incline(PriceList.Cosplay)}", CreateOrderDelegate(PriceList.Cosplay), 
                new string[] { "Имя героя" }, CommandType.Hidden);

        public Command CreateOrderGame()
            => new Command("ЗаказИгра", $"Заказать игру на трансляцию, цена: {PriceList.Game} {currency.Incline(PriceList.Game)}", CreateOrderDelegate(PriceList.Game), 
                new string[] { "Название игры" }, CommandType.Order);

        public Command CreateOrderMovie()
            => new Command("ЗаказФильм", $"Заказать фильм на трансляцию, цена: {PriceList.Movie} {currency.Incline(PriceList.Movie)}", CreateOrderDelegate(PriceList.Movie),
                new string[] { "Название фильма" }, CommandType.Order);

        public Command CreateOrderAnime()
            => new Command("ЗаказАниме", $"Заказать серию аниме на трансляцию, цена: {PriceList.Anime} {currency.Incline(PriceList.Anime)}", CreateOrderDelegate(PriceList.Anime), 
                new string[] { "Название аниме" }, CommandType.Order);

        public Command CreateOrderVip()
            => new Command("ЗаказVIP", $"Купить VIP, цена: {PriceList.VIP} {currency.Incline(PriceList.VIP)}", CreateOrderDelegate(PriceList.VIP, "VIP"), CommandType.Order);

        public Command CreateOrderParty()
            => new Command("ЗаказГруппы", $"Заказать совместную игру со стримером в Dota 2, цена: {PriceList.Group} {currency.Incline(PriceList.Group)}", CreateOrderDelegate(PriceList.Group, "группу"), 
                CommandType.Order);

        public Command CreateOrderBoost()
            => new Command("ЗаказБуст", $"Заказать стримера для подъема вашего рейтинга в Dota 2 на 1 трансляцию (5-6 часов), цена: {PriceList.Boost} {currency.Incline(PriceList.Boost)}", 
                CreateOrderDelegate(PriceList.Boost, "Буст"), CommandType.Order);

        public Command CreateOrderSong()
            => new Command("ЗаказПесня", $"Заказать воспроизведение песни, цена: {PriceList.Song} {currency.Incline(PriceList.Song)}", CreateOrderDelegate(PriceList.Song),
                new string[] { "Ссылка на песню" }, CommandType.Order);

        public void OrderRemove(string orderName, string customer, int price)
            => listOrders.Remove((orderName, customer, price));

        private Action<OnChatCommandReceivedArgs> CreateOrderDelegate(int price, string product)
        {
            return delegate (OnChatCommandReceivedArgs e)
            {
                if (DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) >= price)
                {
                    listOrders.Add((product, e.Command.ChatMessage.DisplayName, price));
                    twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} успешно сделал заказ! {currency.NominativeMultiple.Title()} будут сняты после принятия заказа");
                    updateOrders(listOrders);
                }
                else
                    readyMadePhrases.NoMoney(e.Command.ChatMessage.DisplayName);
            };
        }

        private Action<OnChatCommandReceivedArgs> CreateOrderDelegate(int price)
        {
            return delegate (OnChatCommandReceivedArgs e)
            {
                if (DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) >= price)
                {
                    if (e.Command.ArgumentsAsList.Count != 0)
                    {
                        listOrders.Add((e.Command.ArgumentsAsString, e.Command.ChatMessage.DisplayName, price));
                        twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} успешно сделал заказ!");
                        updateOrders(listOrders);
                    }
                    else
                        readyMadePhrases.IncorrectCommand();
                }
                else
                    readyMadePhrases.NoMoney(e.Command.ChatMessage.DisplayName);
            };
        }

        private Action<OnChatCommandReceivedArgs> CreateCustomOrderDelegate()
        {
            return delegate (OnChatCommandReceivedArgs e)
            {
                if (e.Command.ArgumentsAsList.Count > 1)
                {
                    int temp;
                    if (Int32.TryParse(e.Command.ArgumentsAsList[0], out temp))
                    {
                        CreateOrderDelegate(temp);
                        if (DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) >= temp)
                        {
                            listOrders.Add((e.Command.ArgumentsAsString.Substring(e.Command.ArgumentsAsList[0].Length + 1), e.Command.ChatMessage.DisplayName, temp));
                            twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} успешно сделал заказ!");
                            updateOrders(listOrders);
                        }
                        else
                            readyMadePhrases.NoMoney(e.Command.ChatMessage.DisplayName);
                    }
                    else
                        readyMadePhrases.IncorrectCommand();
                }
            };
        }
    }
}