using System;
using DryIoc;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Api.Helix.Models.Users;
using TwitchLib.Api.V5.Models.Subscriptions;
using TwitchLib.Api;
using TwitchLib.PubSub;
using TwitchLib.Client.Extensions;
using StriBot.Language;
using StriBot.CustomData;
using StriBot.Speakers;
using StriBot.TwitchBot.Interfaces;
using StriBot.Commands;
using StriBot.DryIoc;
using StriBot.Commands.CommonFunctions;

namespace StriBot.TwitchBot.Implementations
{
    public class TwitchBot : ITwitchBot
    {
        public Dictionary<string, Command> Commands { get; set; }
        public CollectionHelper Bosses { get; set; }
        public int Deaths { get; set; } = 0;
        public string TextReminder { get; set; } = string.Empty;
        public Dictionary<string, (int, int)> UsersBetted { get; set; }

        private readonly Currency currency;
        private CurrencyBaseManager currencyBaseManager;
        private ConnectionCredentials connectionCredentials;
        private TwitchClient twitchClient;
        private TwitchAPI api;
        private Random random;
        private CustomArray customArray;
        private Tuple<ChatMessage, int> duelMember;
        private string[] BettingOptions;
        private bool betsProcessing;
        private int betsTimer;
        private double betsCoefficient;
        private int timer;
        private int duelTimer;
        private int toysForSub = 30;
        private int timeoutTime = 120;
        private int timeoutTimeInMinute = 2;
        private int halberdTime = 10;
        private bool chatModeEnabled = false;
        private Action BossUpdate;
        private Action DeathUpdate;
        private TwitchInfo twitchInfo;
        private Speaker speaker;
        private TwitchPubSub twitchPub;

        private ConcurrentDictionary<string, int> HalberdDictionary { get; set; }

        public TwitchBot(Currency currency)
        {
            this.currency = currency;

            customArray = new CustomArray();
            UsersBetted = new Dictionary<string, (int, int)>();
            HalberdDictionary = new ConcurrentDictionary<string, int>();

            twitchInfo = new TwitchInfo();

            connectionCredentials = new ConnectionCredentials(twitchInfo.BotName, twitchInfo.AccessToken);
            random = new Random();
            Bosses = new CollectionHelper();

            twitchClient = new TwitchClient();
            twitchClient.Initialize(connectionCredentials, twitchInfo.Channel);

            twitchClient.OnChatCommandReceived += OnChatCommandReceived;
            twitchClient.OnJoinedChannel += OnJoinedChannel;
            twitchClient.OnNewSubscriber += OnNewSubscriber;
            twitchClient.OnReSubscriber += OnReSubscriber;
            twitchClient.OnGiftedSubscription += OnGiftedSubscription;
            twitchClient.OnRaidNotification += OnRaidNotification;
            twitchClient.OnMessageReceived += OnMessageReceived;
            twitchClient.Connect();

            api = new TwitchAPI();
            api.Settings.ClientId = twitchInfo.ClientId;
            api.Settings.AccessToken = twitchInfo.AccessToken;

            //twitchPub = new TwitchPubSub();
            //twitchPub.OnPubSubServiceConnected += TwitchPub_OnPubSubServiceConnected; ;
            //twitchPub.OnRewardRedeemed += OnRewardRedeemed;
            //twitchPub.OnChannelCommerceReceived += TwitchPub_OnChannelCommerceReceived;
            //twitchPub.ListenToRewards(twitchInfo.Channel);
            //twitchPub.ListenToCommerce(twitchInfo.Channel);
            //twitchPub.Connect();

            speaker = new Speaker();
            speaker.Say("Бот подключился!");

            //ExampleCallsAsync();
        }

        public void SetConstructorSettings(Action bossUpdate, Action deathUpdate)
        {
            BossUpdate = bossUpdate;
            DeathUpdate = deathUpdate;
        }

        private void TwitchPub_OnChannelCommerceReceived(object sender, TwitchLib.PubSub.Events.OnChannelCommerceReceivedArgs e)
        {
            SendMessage($"Тест: произошла коммерция {e.DisplayName} {e.ItemDescription} {e.Username} {e.PurchaseMessage}");
        }

        private void TwitchPub_OnPubSubServiceConnected(object sender, EventArgs e)
        {
            ;
        }

        private void OnRewardRedeemed(object sender, TwitchLib.PubSub.Events.OnRewardRedeemedArgs e)
        {
            SendMessage($"Тест: произошла награда {e.DisplayName} {e.RewardTitle} {e.RewardCost} {e.RewardPrompt} {e.RewardId}");
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if(e.ChatMessage.IsHighlighted)
            {
                speaker.Say(e.ChatMessage.Message);
            }
        }

        private void OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            SendMessage($"{e.ReSubscriber.DisplayName} подписался! PogChamp Срочно плед этому господину! А пока возьми {toysForSub} {currency.Incline(toysForSub, true)} :)");
            DataBase.AddMoneyToUser(e.ReSubscriber.DisplayName, toysForSub);
            speaker.Say("Спасибо за подписку!");
        }

        public void SendMessage(string message)
            => twitchClient.SendMessage(twitchInfo.Channel, $"/me {message}");

        public void TimerTick()
        {
            timer++;

            if (betsProcessing)
            {
                betsTimer++;
                if (betsTimer == 5)
                    StopBetsProcess();
            }

            if (timer == 40)
                currencyBaseManager.DistributionMoney(1, 5);
            if (timer == 15)
                SendMessage("Если увидел крутой момент, запечатли это! Сделай клип! striboF ");
            if (timer == 30)
                SendMessage("У стримера все под контролем! striboPled ");
            if (timer == 45)
                SendMessage("Спасибо за вашу поддержку! HolidaySanta ");

            if (timer % 10 == 0 && !String.IsNullOrEmpty(TextReminder))
                SendMessage("Напоминание: " + TextReminder);

            if (timer == 61)
                timer = 1;

            if (duelMember != null)
                if (duelTimer >= 3)
                {
                    SendMessage($"Дуэль {duelMember.Item1.DisplayName} никто не принял");
                    duelTimer = 0;
                    duelMember = null;
                }
                else
                    duelTimer++;

            foreach (var user in HalberdDictionary)
            {
                HalberdDictionary[user.Key]--;
                if (HalberdDictionary[user.Key] <= 0)
                    HalberdDictionary.TryRemove(user.Key, out _);
            }
        }

        public void Reconnect()
        {
            twitchClient.Disconnect();
            twitchClient.Reconnect();
            speaker.Say("Бот переподключился");
        }

        public void StopBetsProcess()
        {
            betsProcessing = false;
            betsTimer = 0;
            SendMessage("Ставки больше не принимаются");
        }

        public void CreateBets(string[] options)
        {
            BettingOptions = new string[options.Length - 1];
            for (int i = 0; i < BettingOptions.Length; i++)
                BettingOptions[i] = options[i + 1];
            betsProcessing = true;
            betsTimer = 0;
            UsersBetted.Clear();
            betsCoefficient = (options.Length * 0.5);

            SendMessage(String.Format("Время ставок! Коэффициент {0}. Для участия необходимо написать !ставка [номер ставки] [сколько ставите]", betsCoefficient));
            StringBuilder messageBuilder = new StringBuilder(String.Format("{0}: ", options[0]));
            for (int i = 0; i < BettingOptions.Length; i++)
            {
                messageBuilder.Append($"{i} - {BettingOptions[i]}");
            }
            messageBuilder.Remove(messageBuilder.Length - 2, 2);
            SendMessage(messageBuilder.ToString());
        }

        public void SetBetsWinner(int winner)
        {
            try
            {
                foreach (var bet in UsersBetted)
                {
                    if (bet.Value.Item1 == winner)
                        DataBase.AddMoneyToUser(bet.Key, (int)(bet.Value.Item2 * betsCoefficient) + (bet.Value.Item2 * (-1)));
                    else
                        DataBase.AddMoneyToUser(bet.Key, bet.Value.Item2 * (-1));
                }
                SendMessage($"Победила ставка под номером {winner}! В ставках участвовало {UsersBetted.Count} енотов! Вы можете проверить свой запас {currency.GenitiveMultiple}");

                SendMessage("Победили: " + UsersBetted.Where(x => x.Value.Item1 == winner)
                    .Aggregate(new StringBuilder(), (current, next) => current.Append(current.Length == 0 ? string.Empty : ", ").Append($"{next.Key}:{next.Value.Item2}")).ToString());
                SendMessage("Проиграли: " + UsersBetted.Where(x => x.Value.Item1 != winner)
                    .Aggregate(new StringBuilder(), (current, next) => current.Append(current.Length == 0 ? string.Empty : ", ").Append($"{next.Key}:{next.Value.Item2}")).ToString());

                UsersBetted.Clear();
                betsProcessing = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void ExampleCallsAsync()
        {
            //Checks subscription for a specific user and the channel specified
            Subscription subscription = api.V5.Channels.CheckChannelSubscriptionByUserAsync(twitchInfo.ChannelId, twitchInfo.ChannelId).Result;

            //Return bool if channel is online/offline.
            bool isStreaming = api.V5.Streams.BroadcasterOnlineAsync(twitchInfo.ChannelId).Result;

            ////Gets a list of all the subscritions of the specified channel.
            var allSubscriptions = api.V5.Channels.GetAllSubscribersAsync(twitchInfo.ChannelId).Result;

            //Get channels a specified user follows.
            //GetUsersFollowsResponse userFollows = api.Helix.Users.GetUsersFollowsAsync(twitchInfo.ChannelId).Result;

            //Get Specified Channel Follows
            //var channelFollowers = api.V5.Channels.GetChannelFollowersAsync(twitchInfo.ChannelId).Result;

            //Update Channel Title/Game
            //await api.V5.Channels.UpdateChannelAsync("channel_id", "New stream title", "Stronghold Crusader");
            if (isStreaming)
                isStreaming = !isStreaming;
        }

        /// <summary>
        /// e.GiftedSubscription.DisplayName - кто подарил "Добро пожаловать OrloffNY"
        /// e.GiftedSubscription.MsgParamRecipientUserName - кому подарили "Добро пожаловать syndicatereara!"
        /// </summary>
        private void OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            SendMessage($"{e.GiftedSubscription.DisplayName} подарил подписку для {e.GiftedSubscription.MsgParamRecipientUserName}! PogChamp Спасибо большое! Прими нашу небольшую благодарность в качестве {toysForSub} {currency.Incline(toysForSub)}");
            DataBase.AddMoneyToUser(e.GiftedSubscription.DisplayName, toysForSub);
            speaker.Say("Спасибо за подарочную подписку!");
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
            => SendMessage("Бот успешно подключился!");

        private void OnRaidNotification(object sender, OnRaidNotificationArgs e)
        {
            SendMessage($"Нас атакует армия под руководством {e.RaidNotification.DisplayName}! Поднимаем щиты! PurpleStar PurpleStar PurpleStar ");
            speaker.Say("Нас атакуют! Поднимайте щиты!");
        }

        private void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            SendMessage($"{e.Subscriber.DisplayName} подписался! PogChamp Срочно плед этому господину! А пока возьми {toysForSub} {currency.Incline(toysForSub, true)} :)");
            DataBase.AddMoneyToUser(e.Subscriber.DisplayName, toysForSub);
            speaker.Say("Спасибо за подписку!");
        }

        public void CreateCommands()
        {
            
            var container = GlobalContainer.Default;
            var managerMMR = container.Resolve<MMRManager>();
            var readyMadePhrases = container.Resolve<ReadyMadePhrases>();
            var orderManager = container.Resolve<OrderManager>();
            currencyBaseManager = container.Resolve<CurrencyBaseManager>();

            Commands = new Dictionary<string, Command>()
            {
                #region Информационные
                { "команды", new Command("Команды","Ссылка на список команд",
                delegate (OnChatCommandReceivedArgs e) {
                        SendMessage("https://vk.cc/a6Giqf");}, CommandType.Info)},
                { "mmr", managerMMR.CurrentMMR() },
                { "счет", managerMMR.CurrentAccount() },
                { "dotabuff", new Command("Dotabuff","Ссылка на dotabuff",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("https://ru.dotabuff.com/players/113554714"); }, CommandType.Info)},
                { "vk", new Command("Vk","Наша группа в ВКонтакте",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("https://vk.com/stribog45"); }, CommandType.Info)},
                { "youtube", new Command("Youtube","Архив ключевых записей",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("https://www.youtube.com/channel/UCrp75ozt9Spv5k7oVaRd5MQ"); }, CommandType.Info)},
                { "gg", new Command("GoodGame","Ссылка на дополнительный канал на GoodGame",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("https://goodgame.ru/channel/StriBog45/"); }, CommandType.Info)},
                { "discord", new Command("Discord","Наш discord для связи!",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("https://discord.gg/7Z6HGYZ"); }, CommandType.Info)},
                { "steam", new Command("Steam","Ссылка на мой steam",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("https://steamcommunity.com/id/StriBog45"); }, CommandType.Info)},
                { "uptime", new Command("Uptime","Длительность текущей трансляции",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage( $"Трансляция длится: {GetUptime()}"); }, CommandType.Info)},
                #endregion

                #region Интерактив
                { "снежок", new Command("Снежок","Бросает снежок в объект",
                delegate (OnChatCommandReceivedArgs e) {
                    if(String.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        SendMessage(String.Format("{0} бросил снежок и попал в себя!", e.Command.ChatMessage.DisplayName));
                    else
                    {
                        var accuracy = random.Next(0,100);
                        string snowResult = string.Empty;
                        if(accuracy < 10)
                            snowResult = "и... промазал";
                        if(accuracy >= 10 && accuracy <= 20)
                            snowResult = "но цель уклонилась kekw ";
                        if(accuracy > 30)
                            snowResult = String.Format("и попал {0}", customArray.GetHited());
                        SendMessage(String.Format("{0} бросил снежок в {1} {2}", e.Command.ChatMessage.DisplayName, e.Command.ArgumentsAsString,snowResult));
                    }
                }, new string[] {"Объект"}, CommandType.Interactive )},
                { "roll", new Command("Roll","Бросить Roll",
                delegate (OnChatCommandReceivedArgs e) {
                        var accuracy = random.Next(0,100);
                        SendMessage(String.Format("{0} получает число: {1}", e.Command.ChatMessage.DisplayName, accuracy));
                }, new string[] {"Объект"}, CommandType.Interactive )},
                { "подуть", new Command("Подуть","Дует на цель",
                delegate (OnChatCommandReceivedArgs e) {
                    if(String.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        SendMessage(String.Format("{0} подул на свой нос SMOrc !", e.Command.ChatMessage.DisplayName));
                    else
                    {
                        SendMessage(String.Format("{0} подул на {1}, поднимается юбка и мы обнаруживаем {2} {3}! PogChamp ",
                            e.Command.ChatMessage.DisplayName, e.Command.ArgumentsAsString, customArray.GetUnderpantsType(), customArray.GetUnderpantsColor()));
                    }
                }, new string[] {"Цель"}, CommandType.Interactive )},
                { "совместимость", new Command("Совместимость","Проверяет вашу совместимость с объектом",
                delegate (OnChatCommandReceivedArgs e) {
                    if(String.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        SendMessage(String.Format("Совместимость {0} с собой составляет {1}%", e.Command.ChatMessage.DisplayName, random.Next(0,101)));
                    else
                        SendMessage(String.Format("{0} совместим с {1} на {2}%", e.Command.ChatMessage.DisplayName,e.Command.ArgumentsAsString, random.Next(0,101)));
                }, new string[] {"Объект"}, CommandType.Interactive )},
                { "цветы", new Command("Цветы","Дарит букет цветов объекту",
                delegate (OnChatCommandReceivedArgs e) {
                    if(String.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        SendMessage(String.Format("{0} приобрел букет {1} PepoFlower ", e.Command.ChatMessage.DisplayName, customArray.GetBucket()));
                    else
                        SendMessage(String.Format("{0} дарит {1} букет {2} PepoFlower ", e.Command.ChatMessage.DisplayName,e.Command.ArgumentsAsString, customArray.GetBucket()));
                }, new string[] {"Объект"}, CommandType.Interactive )},
                { "люблю", new Command("Люблю","Показывает насколько вы любите объект",
                delegate (OnChatCommandReceivedArgs e) {
                    if(String.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        SendMessage(String.Format("{0} любит себя на {1}% <3 ", e.Command.ChatMessage.DisplayName, random.Next(0,101)));
                    else
                        SendMessage(String.Format("{0} любит {1} на {2}% <3 ", e.Command.ChatMessage.DisplayName, e.Command.ArgumentsAsString, random.Next(0,101)));
                }, new string[] {"Объект"}, CommandType.Interactive )},
                { "duel", new Command("Duel","Вызывает объект на дуэль в доте 1х1",
                delegate (OnChatCommandReceivedArgs e) {
                    if(!String.IsNullOrEmpty(e.Command.ArgumentsAsString))
                    {
                        var duelAccuraccy = random.Next(0,100);
                        string duelResult = string.Empty;
                        if(duelAccuraccy <= 10)
                            duelResult = "Побеждает в сухую! striboTea ";
                        if(duelAccuraccy > 10 && duelAccuraccy <= 20)
                            duelResult = "Проиграл в сухую. striboCry ";
                        if(duelAccuraccy >20 && duelAccuraccy <= 40)
                            duelResult = "Удалось подловить противника с помощью руны. Победа.";
                        if(duelAccuraccy >40 && duelAccuraccy <= 50)
                            duelResult = "Крипы зажали. Неловко. Поражение. CouldYouNot ";
                        if(duelAccuraccy >50 && duelAccuraccy <= 60)
                            duelResult = "Противник проигнорировал дуэль. CouldYouNot ";
                        if(duelAccuraccy >60 && duelAccuraccy <=70)
                            duelResult = "Перефармил противника и сломал башню. Победа. POGGERS ";
                        if(duelAccuraccy >70 && duelAccuraccy <= 80)
                            duelResult = "Похоже противник намного опытнее на этом герое. Поражение.";
                        if(duelAccuraccy >80 && duelAccuraccy <= 90)
                            duelResult = "Боже, что сейчас произошло!? Победа! striboLyc ";
                        if(duelAccuraccy > 90)
                            duelResult = "Жил до конца, умер как герой. Поражение. FeelsRainMan ";
                        SendMessage(String.Format("{0} вызывает {1} на битву 1х1 на {2}! Итог: {3}", e.Command.ChatMessage.DisplayName, e.Command.ArgumentsAsString, Heroes.GetRandomHero(), duelResult));
                    }
                    else
                        SendMessage("Нельзя бросить дуэль самому себе!");
                }, new string[] {"Объект"}, CommandType.Interactive)},
                { "бутерброд", new Command("Бутерброд","Выдает бутерброд тебе или объекту",
                delegate (OnChatCommandReceivedArgs e) {
                        if(String.IsNullOrEmpty( e.Command.ArgumentsAsString))
                            SendMessage(String.Format("Несу {0} для {1}! HahaCat ", Burger.BurgerCombiner(),e.Command.ChatMessage.DisplayName));
                        else
                            SendMessage(String.Format("Несу {0} для {1}! HahaCat ", Burger.BurgerCombiner(),e.Command.ArgumentsAsString));
                }, new string[] {"Объект"}, CommandType.Interactive)},
                { "checkmmr", new Command("CheckMMR","Узнать рейтинг объекта",
                delegate (OnChatCommandReceivedArgs e) {
                    if(String.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        SendMessage(String.Format("Ваш рейтинг: {0}", random.Next(1,7000)));
                    else
                        SendMessage(String.Format("Рейтинг {0}: {1}", e.Command.ArgumentsAsString,random.Next(1,7000)));}, new string[] {"Объект"}, CommandType.Interactive)},
                { "iq", new Command("IQ","Узнать IQ объекта или свой",
                delegate (OnChatCommandReceivedArgs e) {
                    if(String.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        SendMessage(String.Format("Ваш IQ: {0} SeemsGood ", random.Next(1,200)));
                    else
                        SendMessage(String.Format("IQ {0} составляет: {1}! SeemsGood ", e.Command.ArgumentsAsString,random.Next(1,200)));}, new string[] {"Объект"}, CommandType.Interactive)},
                { "шар", new Command("Шар","Шар предсказаний, формулируйте вопрос для ответа \"да\" или \"нет\" ",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage($"Шар говорит... {customArray.GetBallAnswer()}"); },new string[]{"Вопрос" }, CommandType.Interactive)},
                { "монетка", new Command("Монетка","Орел или решка?",
                delegate (OnChatCommandReceivedArgs e) {
                    int coin = random.Next(0,101);
                    if(coin == 100)
                        SendMessage("Бросаю монетку... Ребро POGGERS ");
                    else
                        SendMessage(String.Format("Бросаю монетку... {0}", coin < 50 ? "Орел" : "Решка"));}, CommandType.Interactive)},
                { "размерг", new Command("РазмерГ","Узнать размер вашей груди",
                delegate (OnChatCommandReceivedArgs e) {
                    int size = random.Next(0,7);

                    if(size == 0)
                        SendMessage(String.Format("0 размер... Извините, {0}, а что мерить? kekw ",e.Command.ChatMessage.DisplayName));
                    if(size == 1)
                        SendMessage(String.Format("1 размер... Не переживай {0}, ещё вырастут striboCry ",e.Command.ChatMessage.DisplayName));
                    if(size == 2)
                        SendMessage(String.Format("2 размер... {0}, ваши груди отлично помещаются в ладошки! billyReady ",e.Command.ChatMessage.DisplayName));
                    if(size == 3)
                        SendMessage(String.Format("3 размер... Идеально... Kreygasm , {0} оставьте мне ваш номерок",e.Command.ChatMessage.DisplayName));
                    if(size == 4)
                        SendMessage(String.Format("4 размер... Внимание мужчин к {0} обеспечено striboPled ",e.Command.ChatMessage.DisplayName));
                    if(size == 5)
                        SendMessage(String.Format("5 размер... В грудях {0} можно утонуть счастливым Kreygasm", e.Command.ChatMessage.DisplayName));
                    if(size == 6)
                        SendMessage(String.Format("6 размер... В ваших руках... Кхм, на грудной клетке {0} две убийственные груши", e.Command.ChatMessage.DisplayName));
                }, CommandType.Interactive)},
                { "размерп", new Command("РазмерП","Узнать размер вашего писюна",
                delegate (OnChatCommandReceivedArgs e) {
                    int size = random.Next(10,21);

                    if(size < 13)
                        SendMessage(String.Format("{0} сантиметров... {1}, не переживай, размер не главное! ", e.Command.ChatMessage.DisplayName,size));
                    else if(size == 13)
                        SendMessage(String.Format("13 сантиметров... {0}, поздравляю, у вас стандарт!  striboF ", e.Command.ChatMessage.DisplayName));
                    else if(size == 20)
                        SendMessage(String.Format("20 сантиметров... {0}, вы можете завернуть свой шланг обратно monkaS ", e.Command.ChatMessage.DisplayName));
                    else
                        SendMessage(String.Format("{0} сантиметров... {1}, ваша девушка... или мужчина, будет в восторге! striboTea ", e.Command.ChatMessage.DisplayName,size));
                }, CommandType.Interactive)},
                { "смерть", new Command("Смерть","Добавляет смерть", Role.Moderator,
                delegate (OnChatCommandReceivedArgs e) {
                    Deaths++;
                    DeathUpdate();
                    SendMessage(String.Format("Смертей: {0}", Deaths));
                    SendMessage("▬▬▬▬▬▬▬▬▬▬ஜ۩۞۩ஜ▬▬▬▬▬▬▬▬▬ ……………..............……...Ｙ Ｏ Ｕ Ｄ Ｉ Ｅ Ｄ…….……….........…..… ▬▬▬▬▬▬▬▬▬▬ஜ۩۞۩ஜ▬▬▬▬▬▬▬▬▬"); }, CommandType.Interactive)},
                { "смертей", new Command("Смертей","Показывает количество смертей",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage(String.Format("Смертей: {0}",Deaths)); }, CommandType.Interactive)},
                { "босс", new Command("Босс","Добавляет босса", Role.Moderator,
                delegate (OnChatCommandReceivedArgs e) {
                    Bosses.Add(e.Command.ArgumentsAsString);
                    BossUpdate();
                }, new string[] {"Имя босса"}, CommandType.Interactive )},
                { "напомнить", new Command("Напомнить","Создает напоминалку. При использовании без указания текста, напоминание будет удалено", Role.Moderator,
                delegate (OnChatCommandReceivedArgs e) {
                    TextReminder = e.Command.ArgumentsAsString;
                    if(TextReminder.Length > 0)
                        SendMessage(String.Format("Напоминание: \"{0}\" создано", e.Command.ArgumentsAsString));
                    else
                        SendMessage("Напоминание удалено");
                }, new string[] {"текст"}, CommandType.Interactive )},
                { "боссы", new Command("Боссы","Список убитых боссов!",
                delegate (OnChatCommandReceivedArgs e) {
                    if(Bosses.Count > 0)
                        SendMessage(Bosses.ToString());
                    else
                        SendMessage("Боссов нет"); }, CommandType.Interactive)},
                { "победа", managerMMR.AddWin() },
                { "поражение", managerMMR.AddLose() },

                #endregion

                #region Заказы
                { "заказ", orderManager.CreateOrder() },
                { "заказгерой", orderManager.CreateOrderHero() },
                { "заказкосплей", orderManager.CreateOrderCosplay() },
                { "заказигра", orderManager.CreateOrderGame() },
                { "заказфильм", orderManager.CreateOrderMovie() },
                { "заказаниме", orderManager.CreateOrderAnime() },
                { "заказvip", orderManager.CreateOrderVip() },
                { "заказгруппы", orderManager.CreateOrderParty() },
                { "заказбуст", orderManager.CreateOrderBoost() },
                { "заказпесня", orderManager.CreateOrderMovie() },
                #endregion

                #region DateBase
                { "ставка", new Command("Ставка","Сделать ставку",
                delegate (OnChatCommandReceivedArgs e) {
                    if(betsProcessing)
                    {
                        int numberOfBets = 0;
                        int amountOfBets = 0;
                        if(e.Command.ArgumentsAsList.Count == 2 && Int32.TryParse(e.Command.ArgumentsAsList[0],out numberOfBets) && Int32.TryParse(e.Command.ArgumentsAsList[1],out amountOfBets)
                        && numberOfBets < BettingOptions.Length && amountOfBets > 0)
                        {
                            if(DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) < amountOfBets)
                                SendMessage($"{e.Command.ChatMessage.DisplayName} у тебя недостаточно {currency.GenitiveMultiple} для такой ставки!");
                            else
                            {
                                if(!UsersBetted.ContainsKey(e.Command.ChatMessage.DisplayName))
                                {
                                    UsersBetted.Add(e.Command.ChatMessage.DisplayName, (numberOfBets,amountOfBets));
                                    SendMessage($"{e.Command.ChatMessage.DisplayName} успешно сделал ставку!");
                                }
                                else
                                    SendMessage($"{e.Command.ChatMessage.DisplayName} уже сделал ставку!");
                            }
                        }
                        else
                            SendMessage($"{e.Command.ChatMessage.DisplayName} вы неправильно указали ставку");
                    }
                    else
                        SendMessage("В данный момент ставить нельзя!");
                },
                new string[] {"на что","сколько"}, CommandType.Interactive )},
                { "стащить", currencyBaseManager.CreateStealCurrency() },
                { "вернуть", currencyBaseManager.CreateReturnCurrency() },
                { "добавить", currencyBaseManager.CreateAddCurrency() },
                { "подарить", currencyBaseManager.CreateTransferCurrency() },
                { "изъять", currencyBaseManager.CreateRemoveCurrency() },
                { "заначка", currencyBaseManager.CreateCheckBalance() },
                { "разбросать", currencyBaseManager.CreateDistributeCurrency() },
                { "s", new Command("S", $"Заказ музыки с Youtube или Sound Cloud. Цена: {PriceList.Song} {currency.Incline(PriceList.Song)}",
                delegate (OnChatCommandReceivedArgs e) {

                    if(e.Command.ArgumentsAsList.Count == 1)
                    {
                        var amount = DataBase.CheckMoney(e.Command.ChatMessage.DisplayName);
                        if(amount >= PriceList.Song)
                        {
                            SendMessage(String.Format("!sr {0}", e.Command.ArgumentsAsString));
                            DataBase.AddMoneyToUser(e.Command.ChatMessage.DisplayName,-PriceList.Song);
                        }
                        else
                            readyMadePhrases.NoMoney(e.Command.ChatMessage.DisplayName);
                    }
                    else
                        SendMessage("Нужна ссылка на Sound Cloud");
                }, new string[]{"ссылка"}, CommandType.Hidden)},
                { "дуэль", new Command("Дуэль", $"Дуэль с {currency.InstrumentalMultiple} или без, с timeout, проигравший в дуэли отправляется на {timeoutTime} секунд в timeout",
                delegate (OnChatCommandReceivedArgs e) {
                    int amount = 0;
                    if(duelMember == null)
                    {
                        if(e.Command.ArgumentsAsList.Count > 0)
                        {
                            Int32.TryParse(e.Command.ArgumentsAsString,out amount);
                            if(amount > 0)
                            {
                                if(amount <= DataBase.CheckMoney(e.Command.ChatMessage.DisplayName))
                                {
                                    SendMessage($"Кто осмелится принять вызов {e.Command.ChatMessage.DisplayName} в смертельной дуэли со ставкой в {amount} {currency.Incline(amount, true)}?");
                                    duelMember = new Tuple<ChatMessage, int>(e.Command.ChatMessage,amount);
                                    duelTimer = 0;
                                }
                                else
                                    readyMadePhrases.NoMoney(e.Command.ChatMessage.DisplayName);
                            }
                            else
                                readyMadePhrases.IncorrectCommand();
                        }
                        else
                        {
                            SendMessage(String.Format("Кто осмелится принять вызов {0} в смертельной дуэли?",e.Command.ChatMessage.DisplayName));
                            duelMember = new Tuple<ChatMessage, int>(e.Command.ChatMessage,amount);
                            duelTimer = 0;
                        }

                    }
                    else
                    {
                        if(duelMember.Item1.DisplayName == e.Command.ChatMessage.DisplayName)
                            SendMessage(String.Format("@{0} не торопись! Твоё время ещё не пришло",duelMember.Item1.DisplayName));
                        else if(DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) < duelMember.Item2)
                            readyMadePhrases.NoMoney(e.Command.ChatMessage.DisplayName);
                        else
                        {
                            SendMessage(String.Format("Смертельная дуэль между {0} и {1}!",duelMember.Item1.DisplayName,e.Command.ChatMessage.DisplayName));
                            ChatMessage winner = duelMember.Item1;
                            ChatMessage looser = duelMember.Item1;
                            if(random.Next(2) == 0)
                                winner = e.Command.ChatMessage;
                            else
                                looser = e.Command.ChatMessage;
                            DataBase.AddMoneyToUser(winner.DisplayName,duelMember.Item2);
                            DataBase.AddMoneyToUser(looser.DisplayName,-duelMember.Item2);
                            if(amount != 0)
                                SendMessage($"Дуэлянты достают пистолеты... Выстрел!.. На земле лежит {looser.DisplayName}. {winner.DisplayName} получил за победу {duelMember.Item2} {currency.GenitiveMultiple}! Kappa )/");
                            else
                                SendMessage($"Дуэлянты достают пистолеты... Выстрел!.. На земле лежит {looser.DisplayName}. Поздравляем победителя {winner.DisplayName} Kappa )/");
                            if(!looser.IsModerator)
                                TimeoutUserExt.TimeoutUser(twitchClient, twitchInfo.Channel, looser.Username, new TimeSpan(0, timeoutTimeInMinute, 0), "Ваш противник - (凸ಠ益ಠ)凸");
                            duelMember = null;
                        }
                    }
                }, new string[]{"размер ставки" }, CommandType.Interactive)},
                { "алебарда", new Command("Алебарда",$"Запретить использовать команды на {halberdTime} минут. Цена: {PriceList.Halberd} {currency.GenitiveMultiple}",
                delegate (OnChatCommandReceivedArgs e) {
                    if(e.Command.ArgumentsAsList.Count == 1 )
                    {
                        if (DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) >= PriceList.Halberd)
                        {
                            DataBase.AddMoneyToUser(e.Command.ChatMessage.DisplayName,-PriceList.Halberd);

                            if(HalberdDictionary.ContainsKey(e.Command.ArgumentsAsList[0]))
                                HalberdDictionary[DataBase.CleanNickname(e.Command.ArgumentsAsList[0])] += halberdTime;
                            else
                                HalberdDictionary.TryAdd(DataBase.CleanNickname(e.Command.ArgumentsAsList[0]), halberdTime);

                            SendMessage(String.Format("{0} использовал алебарду на {1}! Цель обезаружена на {2} минут!",e.Command.ChatMessage.DisplayName,e.Command.ArgumentsAsList[0], halberdTime));
                        }
                        else
                            readyMadePhrases.NoMoney(e.Command.ChatMessage.DisplayName);
                    }
                    else
                        readyMadePhrases.IncorrectCommand();
                }, new string[]{"цель"}, CommandType.Interactive)},
                #endregion

                #region Стримеры
                { "daisy", new Command("Daisy","Показывает ссылку на twitch Daisy(roliepolietrolie)",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("Любимая австралийка, обладает хорошим чувством юмора. Не понимает русский, но старается его переводить. А также обожает Dota 2 <3 , twitch.tv/roliepolietrolie"); },CommandType.Streamers)},
                { "katenok", new Command("Katenok","Показывает ссылку на twitch Katenok",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("Очаровашка Катенок(Ffunnya), улыбчивая и светлая персона! Любит DBD и Dota 2 <3 , twitch.tv/katenok"); }, CommandType.Streamers)},
                { "gohapsp",  new Command("Gohapsp","Показывает ссылку на twitch Gohapsp",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("Специалист по хоррорам, twitch.tv/gohapsp"); }, CommandType.Streamers)},
                { "stone", new Command("Stone","Показывает ссылку на twitch Камушка",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("Самый очаровательный камушек! <3 , twitch.tv/sayyees"); }, CommandType.Streamers)},
                { "бескрыл", new Command("Бескрыл","Показывает ссылку на twitch Бескрыл-а",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("Свежие одиночные игры на прохождение :) Добрый, отзывчивый, не оставит без внимания никого! К слову, он разработчик и у него уже есть своя игра! :) twitch.tv/beskr1l_"); }, CommandType.Streamers)},
                { "wlg", new Command("Welovegames","Показывает ссылку на twitch Welovegames",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("Хранитель убежища, харизматичный Денис! Если вы о нём ещё не знаете, крайне рекомендую посмотреть на его деятельность. p.s. обожаю его смайлы. twitch.tv/welovegames"); }, CommandType.Streamers)},
                { "stryk", new Command("StrykOFF","Показывает ссылку на twitch StrykOFF",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("Владелец таверны, создатель лучших шаверм! Для ламповых посиделок :) twitch.tv/strykoff"); }, CommandType.Streamers)},
                { "tilttena", new Command("Tilttena","Показывает ссылку на twitch Tilttena",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("Горящая Алёна!, twitch.tv/tilttena"); }, CommandType.Streamers)},
                { "bezumnaya", new Command("Bezumnaya","Показывает ссылку на twitch Bezumnaya",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("Безумно любит своих зрителей, twitch.tv/bezumnaya"); }, CommandType.Streamers)},
                { "starval", new Command("Starval","Показывает ссылку на twitch Starval",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("Лера. Киев. Стример. :), twitch.tv/starval"); }, CommandType.Streamers)},
                { "aiana", new Command("Aiana","Показывает ссылку на twitch AianaKim",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("Наша улыбашка-очаровашка Аяна BLELELE  twitch.tv/aianakim"); }, CommandType.Streamers)},
                { "reara", new Command("SyndicateReara","Показывает ссылку на twitch SyndicateReara",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("Незабудь выполнить воинское приветствие striboF twitch.tv/syndicatereara"); }, CommandType.Streamers)}
                #endregion
            };
        }
        
        private void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            /*Индентификатор: !,Команда: test, Аргументы: commands hahaha, Сообщение: !test commands hahaha
            twitchClient.SendMessage(TwitchInfo.Channel, String.Format("Индентификатор: {0},Команда: {1}, Аргументы: {2}, Сообщение: {3}", 
                e.Command.CommandIdentifier, e.Command.CommandText, e.Command.ArgumentsAsString, e.Command.ChatMessage.Message));*/
            string lowerCommand = e.Command.CommandText.ToLower();
            if (Commands.ContainsKey(lowerCommand))
            {
                bool canUseCommand = true;

                canUseCommand = CheckRequires(Commands[lowerCommand].Requires, e, canUseCommand);
                canUseCommand = CheckHalberd(e.Command.ChatMessage.DisplayName, canUseCommand);

                if (canUseCommand)
                    Commands[lowerCommand].Action(e);
            }
        }

        private bool CheckRequires(Role role, OnChatCommandReceivedArgs e, bool canUseCommand)
        {
            if (role != Role.Any)
                if (!((role == Role.Subscriber && e.Command.ChatMessage.IsSubscriber) ||
                            (role == Role.Moderator && (e.Command.ChatMessage.IsModerator || e.Command.ChatMessage.IsBroadcaster)) ||
                            (role == Role.Broadcaster && e.Command.ChatMessage.IsBroadcaster)))
                {
                    SendMessage("У вас недостаточно прав!");
                    canUseCommand = false;
                }
            return canUseCommand;
        }

        private bool CheckHalberd(string name, bool canUseCommand)
        {
            if(HalberdDictionary.ContainsKey(name))
            {
                canUseCommand = false;
                SendMessage($"{name} обезаружен ещё {HalberdDictionary[name]} минут(ы,у)!");
            }

            return HalberdDictionary.ContainsKey(name) ? false : canUseCommand;
        }

        public void SmileMode()
        {
            if (chatModeEnabled)
            {
                twitchClient.EmoteOnlyOff(twitchInfo.Channel);
                chatModeEnabled = false;
            }
            else
            {
                twitchClient.EmoteOnlyOn(twitchInfo.Channel);
                chatModeEnabled = true;
            }
        }

        public void SubMode()
        {
            if (chatModeEnabled)
            {
                twitchClient.SubscribersOnlyOff(twitchInfo.Channel);
                chatModeEnabled = false;
            }
            else
            {
                twitchClient.SubscribersOnlyOn(twitchInfo.Channel);
                chatModeEnabled = true;
            }
        }

        public void FollowersMode()
            => twitchClient.FollowersOnlyOn(twitchInfo.Channel, new TimeSpan());

        public void FollowersModeOff()
            => twitchClient.FollowersOnlyOff(twitchInfo.Channel);

        string GetUptime()
        {
            string userId = GetUserId(twitchInfo.Channel);

            return String.IsNullOrEmpty(userId) ? "Offline" : api.V5.Streams.GetUptimeAsync(userId).Result.Value.ToString(@"hh\:mm\:ss");
        }

        string GetUserId(string username)
        {
            var userList = api.V5.Users.GetUserByNameAsync(username).Result.Matches;

            return userList[0]?.Id;
        }
    }
}