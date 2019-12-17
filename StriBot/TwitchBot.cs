using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Api.Helix.Models.Users;
using TwitchLib.Api.V5.Models.Subscriptions;
using TwitchLib.Api;
using TwitchLib.Client.Extensions;

namespace StriBot
{

    public class TwitchBot
    {
        public Dictionary<string, Command> Commands { get; set; }
        public CollectionHelper Bosses { get; set; }
        public int Deaths { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int CoreMMR { get; set; } = 4300;
        public int SupMMR { get; set; } = 3900;
        public string TextReminder { get; set; } = "";
        public Dictionary<string, (int, int)> UsersBetted { get; set; }
        private int DistributionAmountUsers { get; set; }
        private int DistributionAmountPerUsers { get; set; }

        private ConnectionCredentials connectionCredentials;
        private TwitchClient twitchClient;
        private TwitchAPI api;
        private Random random;
        readonly private string[] AnswersOfBall = new string[]{ "Бесспорно", "Разумеется", "Никаких сомнений", "Определённо да", "Можешь быть уверен в этом", "Мне кажется — «да»", "Вероятнее всего", "Хорошие перспективы", "Знаки говорят — «да»",
            "Да", "Пока не ясно, попробуй снова", "Спроси позже", "Лучше не рассказывать", "Сейчас нельзя предсказать", "Сконцентрируйся и спроси опять", "Даже не думай", "Мой ответ — «нет»", "По моим данным — «нет»",
            "Перспективы не очень хорошие", "Весьма сомнительно" };
        readonly private string[] Hited = new string[] { "в ухо", "по попке PogChamp ", "в плечо", "в пузо", "в ноутбук", "в спину", "в глаз и оставил фингал" };
        readonly private string[] UnderpantsType = new string[] { "Слипы", "Тонг", "Танга", "Панталоны", "Бикини", "Бразилиано", "Шорты", "Мини-стринги", "Классические трусы", "Хипсстерсы" };
        readonly private string[] UnderpantsColor = new string[] { "синие", "красные", "желтые", "черные", "розовые", "зеленые", "в горошек", "в цветочек", "в полоску", "прозрачные", "львица", "тигрица", "слоник", "армейка" };
        readonly private string[] Bucket = new string[] { "Ромашек", "Тюльпанов", "Алых роз", "Белых роз", "Гладиолусов", "Лилий", "Калл" };
        private Tuple<ChatMessage, int> duelMember;
        private List<string> ReceivedUsers;
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
        private int subCoefficient = 2;
        private int SubCoefficient { get => subBonus ? subCoefficient : 1; }
        private bool subBonus;
        private string medallion = "Властелин 3";
        private Action<List<(string, string, int)>> OrdersUpdate;
        private Action BossUpdate;
        private TwitchInfo twitchInfo;
        public List<(string, string, int)> ListOrders { get; set; }
        private ConcurrentDictionary<string, int> HalberdDictionary { get; set; }

        public TwitchBot(Action<List<(string, string, int)>> ordersUpdate, Action bossUpdate)
        {
            OrdersUpdate = ordersUpdate;
            BossUpdate = bossUpdate;
            CreateCommands();

            ReceivedUsers = new List<string>();
            UsersBetted = new Dictionary<string, (int, int)>();
            ListOrders = new List<(string, string, int)>();
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
            twitchClient.OnRaidNotification += OnRaidNotification;
            twitchClient.OnGiftedSubscription += TwitchClient_OnGiftedSubscription;
            twitchClient.Connect();

            api = new TwitchAPI();
            api.Settings.ClientId = twitchInfo.ClientId;
            api.Settings.AccessToken = twitchInfo.AccessToken;

            //ExampleCallsAsync();

        }

        public void SendMessage(string message)
        {
            twitchClient.SendMessage(twitchInfo.Channel, message);
        }

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
                DistributionMoney(1, 5);
            if (timer == 15)
                SendMessage("Если увидел крутой момент, запечатли это! Сделай клип! striboS ");
            if (timer == 30)
                SendMessage("У стримера все под контролем! wlgDen ");
            if (timer == 45)
                SendMessage("Спасибо за вашу поддержку! striboPled ");

            if (timer % 10 == 0 && !String.IsNullOrEmpty(TextReminder))
                SendMessage("Напоминание: " + TextReminder);

            if (timer == 61)
                timer = 1;

            if (duelMember != null)
                if (duelTimer >= 3)
                {
                    SendMessage(String.Format("Дуэль {0} никто не принял", duelMember.Item1.DisplayName));
                    duelTimer = 0;
                    duelMember = null;
                }
                else
                    duelTimer++;

            foreach (var user in HalberdDictionary)
            {
                HalberdDictionary[user.Key]--;
                if (HalberdDictionary[user.Key] <= 0)
                    HalberdDictionary.TryRemove(user.Key, out int test);
            }
        }

        public void Reconnect()
        {
            twitchClient.Disconnect();
            twitchClient.Reconnect();
        }

        public void StopBetsProcess()
        {
            betsProcessing = false;
            betsTimer = 0;
            SendMessage("Ставки больше не принимаются");
        }

        public void DistributionMoney(int perUser, int maxUsers, bool bonus = true)
        {
            subBonus = bonus;
            DistributionAmountPerUsers = perUser;
            DistributionAmountUsers = maxUsers;
            SendMessage("Замечены игрушки без присмотра! Время полоскать! Пиши !стащить wlgF ");
            ReceivedUsers.Clear();
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
                messageBuilder.Append(String.Format("{0} - {1}, ", i, BettingOptions[i]));
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
                SendMessage(String.Format("Победила ставка под номером {0}! В ставках участвовало {1} енотов! Вы можете проверить свой запас игрушек", winner, UsersBetted.Count));

                SendMessage("Победили: " + UsersBetted.Where(x => x.Value.Item1 == winner)
                    .Aggregate(new StringBuilder(), (current, next) => current.Append(current.Length == 0 ? "" : ", ").Append($"{next.Key}:{next.Value.Item2}")).ToString());
                SendMessage("Проиграли: " + UsersBetted.Where(x => x.Value.Item1 != winner)
                    .Aggregate(new StringBuilder(), (current, next) => current.Append(current.Length == 0 ? "" : ", ").Append($"{next.Key}:{next.Value.Item2}")).ToString());

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

        private void TwitchClient_OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            SendMessage(String.Format("Добро пожаловать {0}! striboPled ", e.GiftedSubscription.DisplayName));
            SendMessage(String.Format("Добро пожаловать {0}! striboPled ", e.GiftedSubscription.MsgParamRecipientUserName));
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            twitchClient.SendMessage(e.Channel, "Бот успешно подключился!");
        }

        private void OnRaidNotification(object sender, OnRaidNotificationArgs e)
        {
            SendMessage(String.Format("Нас атакует армия под руководством {0}! Поднимаем щиты! PurpleStar PurpleStar PurpleStar ", e.RaidNotification.DisplayName));
        }

        private void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            SendMessage(String.Format("Добро пожаловать {0}! Срочно плед этому господину! striboPled Вам начислено {1} игрушек!", e.Subscriber.DisplayName, toysForSub));
            DataBase.AddMoneyToUser(e.Subscriber.DisplayName, toysForSub);
        }

        private void CreateCommands()
        {
            Commands = new Dictionary<string, Command>()
            {
                #region Информационные
                {"команды", new Command("Команды","Ссылка на список команд",
                delegate (OnChatCommandReceivedArgs e) {
                        SendMessage("https://vk.cc/a6Giqf");}, CommandType.Info)},
                {"mmr", new Command("mmr","Узнать рейтинг стримера в Dota 2",
                delegate (OnChatCommandReceivedArgs e) {
                        SendMessage(String.Format("Основа: {0} Поддержка: {1} Звание: {2}", CoreMMR, SupMMR, medallion));}, CommandType.Info)},
                {"dotabuff", new Command("Dotabuff","Ссылка на dotabuff",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("https://ru.dotabuff.com/players/113554714"); }, CommandType.Info)},
                {"vk", new Command("Vk","Наша группа в ВКонтакте",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("https://vk.com/stribog45"); }, CommandType.Info)},
                {"youtube", new Command("Youtube","Архив ключевых записей",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("https://www.youtube.com/channel/UCrp75ozt9Spv5k7oVaRd5MQ"); }, CommandType.Info)},
                {"gg", new Command("GoodGame","Ссылка на дополнительный канал на GoodGame",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("https://goodgame.ru/channel/StriBog45/"); }, CommandType.Info)},
                {"discord", new Command("Discord","Наш discord для связи!",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("https://discord.gg/7Z6HGYZ"); }, CommandType.Info)},
                {"steam", new Command("Steam","Ссылка на мой steam",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("https://steamcommunity.com/id/StriBog45"); }, CommandType.Info)},
                #endregion

                #region Интерактив
                { "снежок", new Command("Снежок","Бросает снежок в объект",
                delegate (OnChatCommandReceivedArgs e) {
                    if(String.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        SendMessage(String.Format("{0} бросил снежок и попал в себя!", e.Command.ChatMessage.DisplayName));
                    else
                    {
                        var accuracy = random.Next(0,100);
                        string snowResult = "";
                        if(accuracy < 10)
                            snowResult = "и... промазал";
                        if(accuracy >= 10 && accuracy <= 20)
                            snowResult = "но цель уклонилась striboPledik";
                        if(accuracy > 30)
                            snowResult = String.Format("и попал {0}",RandomHelper.GetRandomOfArray(Hited));
                        SendMessage(String.Format("{0} бросил снежок в {1} {2}", e.Command.ChatMessage.DisplayName, e.Command.ArgumentsAsString,snowResult));
                    }
                },
                new string[] {"Объект"}, CommandType.Interactive )},
                {"roll", new Command("Roll","Бросить Roll",
                delegate (OnChatCommandReceivedArgs e) {
                        var accuracy = random.Next(0,100);
                        SendMessage(String.Format("{0} получает число: {1}", e.Command.ChatMessage.DisplayName, accuracy));
                },
                new string[] {"Объект"}, CommandType.Interactive )},
                { "подуть", new Command("Подуть","Дует на цель",
                delegate (OnChatCommandReceivedArgs e) {
                    if(String.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        SendMessage(String.Format("{0} подул на свой нос SMOrc !", e.Command.ChatMessage.DisplayName));
                    else
                    {
                        SendMessage(String.Format("{0} подул на {1}, поднимается юбка и мы обнаруживаем {2} {3}! PogChamp ",
                            e.Command.ChatMessage.DisplayName, e.Command.ArgumentsAsString,RandomHelper.GetRandomOfArray(UnderpantsType),RandomHelper.GetRandomOfArray(UnderpantsColor)));
                    }
                },
                new string[] {"Цель"}, CommandType.Interactive )},
                { "совместимость", new Command("Совместимость","Проверяет вашу совместимость с объектом",
                delegate (OnChatCommandReceivedArgs e) {
                    if(String.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        SendMessage(String.Format("Совместимость {0} с собой составляет {1}%", e.Command.ChatMessage.DisplayName, random.Next(0,101)));
                    else
                        SendMessage(String.Format("{0} совместим с {1} на {2}%", e.Command.ChatMessage.DisplayName,e.Command.ArgumentsAsString, random.Next(0,101)));
                },
                new string[] {"Объект"}, CommandType.Interactive )},
                { "цветы", new Command("Цветы","Дарит букет цветов объекту",
                delegate (OnChatCommandReceivedArgs e) {
                    if(String.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        SendMessage(String.Format("{0} приобрел букет {1} wlgFlowers ", e.Command.ChatMessage.DisplayName, RandomHelper.GetRandomOfArray(Bucket)));
                    else
                        SendMessage(String.Format("{0} дарит {1} букет {2} wlgFlowers ", e.Command.ChatMessage.DisplayName,e.Command.ArgumentsAsString, RandomHelper.GetRandomOfArray(Bucket)));
                },
                new string[] {"Объект"}, CommandType.Interactive )},
                { "люблю", new Command("Люблю","Показывает насколько вы любите объект",
                delegate (OnChatCommandReceivedArgs e) {
                    if(String.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        SendMessage(String.Format("{0} любит себя на {1}% <3 ", e.Command.ChatMessage.DisplayName, random.Next(0,101)));
                    else
                        SendMessage(String.Format("{0} любит {1} на {2}% <3 ", e.Command.ChatMessage.DisplayName,e.Command.ArgumentsAsString, random.Next(0,101)));
                },
                new string[] {"Объект"}, CommandType.Interactive )},
                {"duel", new Command("Duel","Вызывает объект на дуэль в доте 1х1",
                delegate (OnChatCommandReceivedArgs e) {
                    if(!String.IsNullOrEmpty(e.Command.ArgumentsAsString))
                    {
                        var duelAccuraccy = random.Next(0,100);
                        string duelResult = "";
                        if(duelAccuraccy <= 10)
                            duelResult = "Побеждает в сухую! wlgEz ";
                        if(duelAccuraccy > 10 && duelAccuraccy <= 20)
                            duelResult = "Проиграл в сухую. wlgCry ";
                        if(duelAccuraccy >20 && duelAccuraccy <= 40)
                            duelResult = "Удалось подловить противника с помощью руны. Победа.";
                        if(duelAccuraccy >40 && duelAccuraccy <= 50)
                            duelResult = "Крипы зажали. Неловко. Поражение.";
                        if(duelAccuraccy >50 && duelAccuraccy <= 60)
                            duelResult = "Противник проигнорировал дуэль.";
                        if(duelAccuraccy >60 && duelAccuraccy <=70)
                            duelResult = "Перефармил противника и сломал башню. Победа.";
                        if(duelAccuraccy >70 && duelAccuraccy <= 80)
                            duelResult = "Похоже противник намного опытнее на этом герое. Поражение.";
                        if(duelAccuraccy >80 && duelAccuraccy <= 90)
                            duelResult = "Боже, что сейчас произошло!? Победа! TearGlove ";
                        if(duelAccuraccy > 90)
                            duelResult = "Жил до конца, умер как герой. Поражение. wlgSad ";
                        SendMessage(String.Format("{0} вызывает {1} на битву 1х1 на {2}! Итог: {3}", e.Command.ChatMessage.DisplayName, e.Command.ArgumentsAsString, Heroes.GetRandomHero(), duelResult));
                    }
                    else
                        SendMessage("Нельзя бросить дуэль самому себе!");
                },
                new string[] {"Объект"}, CommandType.Interactive)},
                { "бутерброд", new Command("Бутерброд","Выдает бутерброд тебе или объекту",
                delegate (OnChatCommandReceivedArgs e) {
                        if(String.IsNullOrEmpty( e.Command.ArgumentsAsString))
                            SendMessage(String.Format("Несу {0} для {1}! wlgMug ", Burger.BurgerCombiner(),e.Command.ChatMessage.DisplayName));
                        else
                            SendMessage(String.Format("Несу {0} для {1}! wlgMug ", Burger.BurgerCombiner(),e.Command.ArgumentsAsString));
                },new string[] {"Объект"}, CommandType.Interactive)},
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
                    SendMessage(String.Format("Шар говорит... {0}", RandomHelper.GetRandomOfArray(AnswersOfBall))); },new string[]{"Вопрос" }, CommandType.Interactive)},
                { "монетка", new Command("Монетка","Орел или решка?",
                delegate (OnChatCommandReceivedArgs e) {
                    int coin = random.Next(0,101);
                    if(coin == 100)
                        SendMessage("Бросаю монетку... Ребро wlgH ");
                    else
                        SendMessage(String.Format("Бросаю монетку... {0}", coin < 50 ? "Орел" : "Решка"));}, CommandType.Interactive)},
                { "размерг", new Command("РазмерГ","Узнать размер вашей груди",
                delegate (OnChatCommandReceivedArgs e) {
                    int size = random.Next(0,7);

                    if(size == 0)
                        SendMessage(String.Format("0 размер... Извините, {0}, а что мерить? wlgThonk ",e.Command.ChatMessage.DisplayName));
                    if(size == 1)
                        SendMessage(String.Format("1 размер... Не переживай {0}, ещё вырастут striboPled",e.Command.ChatMessage.DisplayName));
                    if(size == 2)
                        SendMessage(String.Format("2 размер... {0}, ваши груди отлично помещаются в ладошки! wlgF ",e.Command.ChatMessage.DisplayName));
                    if(size == 3)
                        SendMessage(String.Format("3 размер... Идеально... KreyGasm , {0} оставьте мне ваш номерок",e.Command.ChatMessage.DisplayName));
                    if(size == 4)
                        SendMessage(String.Format("4 размер... Внимание мужчин к {0} обеспечено wlgDen ",e.Command.ChatMessage.DisplayName));
                    if(size == 5)
                        SendMessage(String.Format("5 размер... В грудях {0} можно утонуть счастливым KreyGasm", e.Command.ChatMessage.DisplayName));
                    if(size == 6)
                        SendMessage(String.Format("6 размер... В ваших руках... Кхм, на грудной клетке {0} две убийственные груши", e.Command.ChatMessage.DisplayName));
                }, CommandType.Interactive)},
                { "размерп", new Command("РазмерП","Узнать размер вашего писюна",
                delegate (OnChatCommandReceivedArgs e) {
                    int size = random.Next(10,21);

                    if(size < 13)
                        SendMessage(String.Format("{0} сантиметров... {1}, не переживай, размер не главное! ",e.Command.ChatMessage.DisplayName,size));
                    else if(size == 13)
                        SendMessage(String.Format("13 сантиметров... {0}, поздравляю, у вас стандарт!  wlgF ",e.Command.ChatMessage.DisplayName));
                    else if(size == 20)
                        SendMessage(String.Format("20 сантиметров... {0}, вы можете завернуть свой шланг обратно wlgScared ",e.Command.ChatMessage.DisplayName));
                    else
                        SendMessage(String.Format("{0} сантиметров... {1}, ваша девушка... или мужчина, будет в восторге! wlgAsm ",e.Command.ChatMessage.DisplayName,size));
                }, CommandType.Interactive)},
                { "смерть", new Command("Смерть","Добавляет смерть", Role.Moderator,
                delegate (OnChatCommandReceivedArgs e) {
                    Deaths++;
                    SendMessage(String.Format("Смертей: {0}", Deaths));
                    SendMessage("▬▬▬▬▬▬▬▬▬▬ஜ۩۞۩ஜ▬▬▬▬▬▬▬▬▬ …………………...Ｙ Ｏ Ｕ Ｄ Ｉ Ｅ Ｄ………………… ▬▬▬▬▬▬▬▬▬▬ஜ۩۞۩ஜ▬▬▬▬▬▬▬▬▬"); }, CommandType.Interactive)},
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
                { "победа", new Command("Победа","Добавляет победу", Role.Moderator,
                delegate (OnChatCommandReceivedArgs e) {
                    Wins++;
                    CoreMMR += 25;
                    SendMessage(String.Format("Побед: {0}, Поражений: {1}", Wins, Losses)); }, CommandType.Interactive)},
                { "поражение", new Command("Поражение","Добавляет поражение", Role.Moderator,
                delegate (OnChatCommandReceivedArgs e) {
                    Losses++;
                    CoreMMR -= 25;
                    SendMessage(String.Format("Побед: {0}, Поражений: {1}", Wins, Losses)); }, CommandType.Interactive)},
                { "счет", new Command("Счет","Текущий счет побед и поражений",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage(String.Format("Побед: {0}, Поражений: {1}", Wins, Losses)); }, CommandType.Interactive)},
                #endregion

                #region Заказы
                { "заказ", new Command("Заказ",String.Format("Предложить свой заказ",PriceList.Hero),CreateOrder(), new string[] {"Игрушки", "Заказ"}, CommandType.Order )},
                { "заказгерой", new Command("ЗаказГерой",String.Format("Заказать героя на игру, цена: {0} игрушек",PriceList.Hero),CreateOrder(PriceList.Hero), new string[] {"Имя героя"}, CommandType.Order )},
                { "заказкосплей", new Command("ЗаказКосплей",String.Format("Заказать косплей на трансляцию, цена: {0} игрушек",PriceList.Cosplay),CreateOrder(PriceList.Cosplay), new string[] {"Имя героя"}, CommandType.Hidden )},
                { "заказигра", new Command("ЗаказИгры",String.Format("Заказать игру на трансляцию, цена: {0} игрушек",PriceList.Game),CreateOrder(PriceList.Game), new string[] {"Название игры"}, CommandType.Order )},
                { "заказvip", new Command("ЗаказVIP",String.Format("Купить VIP, цена: {0} игрушек",PriceList.VIP),CreateOrder(PriceList.VIP, "VIP"), CommandType.Order)},
                { "заказгруппы", new Command("ЗаказГруппы",String.Format("Заказать совместную игру со стримером, цена: {0} игрушек",PriceList.Group),CreateOrder(PriceList.Group, "Group"), CommandType.Order)},
                { "заказбуст", new Command("ЗаказБуст",String.Format("Заказать буст, 1 трансляция, цена: {0} игрушек",PriceList.Boost),CreateOrder(PriceList.Boost, "Буст"), CommandType.Order)},
                { "заказпесня", new Command("ЗаказПесня",String.Format("Заказать воспроизведение песни, цена: {0} игрушек",PriceList.Song),CreateOrder(PriceList.Song), new string[] {"Ссылка на песню"}, CommandType.Order )},
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
                                SendMessage(String.Format("{0} у тебя недостаточно игрушек для такой ставки!",e.Command.ChatMessage.DisplayName));
                            else
                            {
                                if(!UsersBetted.ContainsKey(e.Command.ChatMessage.DisplayName))
                                {
                                    UsersBetted.Add(e.Command.ChatMessage.DisplayName, (numberOfBets,amountOfBets));
                                    SendMessage(String.Format("{0} успешно сделал ставку!",e.Command.ChatMessage.DisplayName));
                                }
                                else
                                    SendMessage(String.Format("{0} уже сделал ставку!",e.Command.ChatMessage.DisplayName));
                            }
                        }
                        else
                            SendMessage(String.Format("{0} вы неправильно указали ставку",e.Command.ChatMessage.DisplayName));
                    }
                    else
                        SendMessage("В данный момент ставить нельзя!");
                },
                new string[] {"на что","сколько"}, CommandType.Interactive )},
                { "стащить", new Command("Стащить","Крадет игрушку без присмотра",
                delegate (OnChatCommandReceivedArgs e) {
                    if(DistributionAmountUsers > 0)
                    {
                        if( ReceivedUsers.Where(x => x.CompareTo(e.Command.ChatMessage.DisplayName) == 0).ToArray().Count() == 0)
                        {
                            if(e.Command.ChatMessage.IsSubscriber)
                                DataBase.AddMoneyToUser(e.Command.ChatMessage.DisplayName, DistributionAmountPerUsers*SubCoefficient);
                            else
                                DataBase.AddMoneyToUser(e.Command.ChatMessage.DisplayName, DistributionAmountPerUsers);
                            SendMessage(String.Format("{0} успешно стащил игрушку!", e.Command.ChatMessage.DisplayName));
                            DistributionAmountUsers--;
                            ReceivedUsers.Add(e.Command.ChatMessage.DisplayName);
                        }
                        else
                            SendMessage(String.Format("{0} вы уже забрали игрушку! Не жадничайте!", e.Command.ChatMessage.DisplayName));
                    }
                    else
                    {
                        SendMessage(String.Format("{0} игрушек не осталось!", e.Command.ChatMessage.DisplayName));
                    }}, CommandType.Interactive)},
                { "вернуть", new Command("Вернуть","Возвращает игрушку боту",
                delegate (OnChatCommandReceivedArgs e) {
                    if(DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) > 0)
                    {
                            if(DistributionAmountPerUsers == 0)
                                DistributionAmountPerUsers = 1;
                            if(e.Command.ChatMessage.IsSubscriber)
                                DataBase.AddMoneyToUser(e.Command.ChatMessage.DisplayName, -DistributionAmountPerUsers*SubCoefficient);
                            else
                                DataBase.AddMoneyToUser(e.Command.ChatMessage.DisplayName, -DistributionAmountPerUsers);
                            SendMessage(String.Format("{0} незаметно вернул игрушку!", e.Command.ChatMessage.DisplayName));
                            DistributionAmountUsers++;
                            ReceivedUsers.Remove(e.Command.ChatMessage.DisplayName);
                    }
                    else
                    {
                        SendMessage(String.Format("{0} у вас нет игрушек!", e.Command.ChatMessage.DisplayName));
                    }}, CommandType.Interactive)},
                { "добавить", new Command("Добавить","Добавить объекту Х игрушек. Только для владельца канала", Role.Broadcaster,
                delegate (OnChatCommandReceivedArgs e) {
                    if(e.Command.ArgumentsAsList.Count == 2)
                    {
                        DataBase.AddMoneyToUser(e.Command.ArgumentsAsList[0],Convert.ToInt32(e.Command.ArgumentsAsList[1]));
                        SendMessage(String.Format("Вы успешно добавили игрушки! wlgF"));
                    }
                    else
                        SendMessage("Неправильная команда");
                    }, new string[]{"объект","количество"},CommandType.Interactive)},
                { "изъять", new Command("Изъять","Изымает объект Х игрушек", Role.Moderator,
                delegate (OnChatCommandReceivedArgs e) {
                    if(e.Command.ArgumentsAsList.Count == 2 && Convert.ToInt32(e.Command.ArgumentsAsList[1]) > 0)
                    {
                        DataBase.AddMoneyToUser(e.Command.ArgumentsAsList[0],Convert.ToInt32(e.Command.ArgumentsAsList[1])*(-1));
                        SendMessage(String.Format("Вы успешно изъяли игрушки! wlgEz "));
                    }
                    else
                        SendMessage("Неправильная команда");
                    }, new string[]{"объект","количество"}, CommandType.Interactive)},
                { "заначка", new Command("Заначка","Текущие количество игрушек у вас",
                delegate (OnChatCommandReceivedArgs e) {
                    if(e.Command.ArgumentsAsList.Count == 0)
                    {
                        var amount = DataBase.CheckMoney(e.Command.ChatMessage.DisplayName);
                        SendMessage(String.Format("{0} имеет {1} игрушек!", e.Command.ChatMessage.DisplayName, amount));
                    }
                    else
                    {
                        var amount = DataBase.CheckMoney(e.Command.ArgumentsAsString);
                        SendMessage(String.Format("{0} имеет {1} игрушек!", e.Command.ArgumentsAsString, amount));
                    }
                }, CommandType.Interactive)},
                { "s", new Command("S",String.Format("Заказ музыки с Youtube или Sound Cloud. Цена: {0} игрушка", PriceList.Song),
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
                            SendMessage(String.Format("{0} у вас недостаточно игрушек! wlgCry", e.Command.ChatMessage.DisplayName));
                    }
                    else
                        SendMessage("Нужна ссылка на Sound Cloud");
                }, new string[]{"ссылка"}, CommandType.Hidden)},
                { "дуэль", new Command("Дуэль",String.Format("Дуэль с игрушками или без, с timeout, проигравший в дуэли отправляется на {0} секунд в timeout",timeoutTime),
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
                                    SendMessage(String.Format("Кто осмелится принять вызов {0} в смертельной дуэли со ставкой в {1} игрушек?",e.Command.ChatMessage.DisplayName, amount));
                                    duelMember = new Tuple<ChatMessage, int>(e.Command.ChatMessage,amount);
                                    duelTimer = 0;
                                }
                                else
                                    SendMessage("У вас недостаточно игрушек для такой ставки!");
                            }
                            else
                                SendMessage("Вы неправильно пользуетесь командой!");
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
                            SendMessage("У вас недостаточно игрушек, чтобы принять дуэль!");
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
                                SendMessage(String.Format("Дуэлянты достают пистолеты... Выстрел!.. На земле лежит {0}. {1} получил за победу {2} игрушек! Kappa )/", looser.DisplayName,winner.DisplayName,duelMember.Item2));
                            else
                                SendMessage(String.Format("Дуэлянты достают пистолеты... Выстрел!.. На земле лежит {0}. Поздравляем победителя {1} Kappa )/", looser.DisplayName, winner.DisplayName));
                            if(!looser.IsModerator)
                                TimeoutUserExt.TimeoutUser(twitchClient, twitchInfo.Channel, looser.DisplayName, new TimeSpan(0, timeoutTimeInMinute, 0), "(凸ಠ益ಠ)凸 - выражение лица вашего противника");
                                //SendMessage(String.Format("/timeout @{0} {1}",looser.DisplayName, timeoutTime));
                            duelMember = null;
                        }
                    }
                }, new string[]{"размер ставки" }, CommandType.Interactive)},
                { "подарить", new Command("Подарить","Подарить игрушки [человек] [игрушек] ",
                delegate (OnChatCommandReceivedArgs e) {
                    int amount = 0;
                    if(e.Command.ArgumentsAsList.Count == 2 && Int32.TryParse(e.Command.ArgumentsAsList[1],out amount) && amount > 0)
                    {
                        if (DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) >= amount)
                        {
                            DataBase.AddMoneyToUser(e.Command.ChatMessage.DisplayName,-amount);
                            DataBase.AddMoneyToUser(e.Command.ArgumentsAsList[0],amount);
                            SendMessage(String.Format("{0} подарил игрушки {1} в количестве {2} ! ",e.Command.ChatMessage.DisplayName,e.Command.ArgumentsAsList[0],amount));
                        }
                        else
                            SendMessage("У вас недостаточно игрушек!");
                    }
                    else
                        SendMessage("Вы неправильно пользуетесь командой!");
                }, new string[]{"кому", "сколько" }, CommandType.Interactive)},
                { "алебарда", new Command("Алебарда",$"Запретить использовать команды на {halberdTime} минут. Цена: {PriceList.Halberd} игрушек",
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
                            SendMessage("У вас недостаточно игрушек!");
                    }
                    else
                        SendMessage("Вы неправильно пользуетесь командой!");
                }, new string[]{"цель"}, CommandType.Interactive)},
                { "разбросать", new Command("Разбросать","Разбрасывает игрушки в чате, любой желающий может стащить",
                delegate (OnChatCommandReceivedArgs e) {
                    if(e.Command.ArgumentsAsList.Count == 2
                        && Int32.TryParse(e.Command.ArgumentsAsList[0], out int amountForPer)
                        && Int32.TryParse(e.Command.ArgumentsAsList[1], out int amountPeople)
                        && amountForPer > 0
                        && amountPeople > 0)
                    {
                        if(DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) >= amountForPer*amountPeople)
                        {
                            DataBase.AddMoneyToUser(e.Command.ChatMessage.DisplayName, amountForPer*amountPeople*(-1));
                            DistributionMoney(amountForPer, amountPeople, false);
                        }
                        else
                            SendMessage("Недостаточно игрушек");
                    }
                    else
                        SendMessage("Некорректное использование команды");

                }, new string[] {"Сколько стащит за раз", "Сколько человек сможет стащить"}, CommandType.Interactive)},
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
                { "stone", new Command("Камушек","Показывает ссылку на twitch Камушка",
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
                { "tilttena", new Command("Tilttena","Показывает ссылку на twitch Tilltena",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("Горящая Алёна!, twitch.tv/tilltena"); }, CommandType.Streamers)},
                { "bezumnaya", new Command("Bezumnaya","Показывает ссылку на twitch Bezumnaya",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("Безумно любит своих зрителей, twitch.tv/bezumnaya"); }, CommandType.Streamers)},
                { "starval", new Command("Starval","Показывает ссылку на twitch Starval",
                delegate (OnChatCommandReceivedArgs e) {
                    SendMessage("Лера. Киев. Стример. :), twitch.tv/starval"); }, CommandType.Streamers)}
                #endregion
            };
        }

        private Action<OnChatCommandReceivedArgs> CreateOrder(int price, string product)
        {
            return delegate (OnChatCommandReceivedArgs e)
            {
                if (DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) >= price)
                {
                    ListOrders.Add((product, e.Command.ChatMessage.DisplayName, price));
                    SendMessage(String.Format("{0} успешно сделал заказ! Игрушки будут сняты после принятия заказа", e.Command.ChatMessage.DisplayName));
                    OrdersUpdate(ListOrders);
                }
                else
                    SendMessage(String.Format("{0} у тебя недостаточно игрушек!", e.Command.ChatMessage.DisplayName));
            };
        }

        private Action<OnChatCommandReceivedArgs> CreateOrder(int price)
        {
            return delegate (OnChatCommandReceivedArgs e)
            {
                if (DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) >= price)
                {
                    if (e.Command.ArgumentsAsList.Count != 0)
                    {
                        ListOrders.Add((e.Command.ArgumentsAsString, e.Command.ChatMessage.DisplayName, price));
                        SendMessage(String.Format("{0} успешно сделал заказ!", e.Command.ChatMessage.DisplayName));
                        OrdersUpdate(ListOrders);
                    }
                    else
                        SendMessage("Некорректное использование команды");
                }
                else
                    SendMessage(String.Format("{0} у тебя недостаточно игрушек!", e.Command.ChatMessage.DisplayName));
            };
        }

        private Action<OnChatCommandReceivedArgs> CreateOrder()
        {
            return delegate (OnChatCommandReceivedArgs e)
            {
                if (e.Command.ArgumentsAsList.Count > 1)
                {
                    int temp;
                    if (Int32.TryParse(e.Command.ArgumentsAsList[0], out temp))
                    {
                        CreateOrder(temp);
                        if (DataBase.CheckMoney(e.Command.ChatMessage.DisplayName) >= temp)
                        {
                            ListOrders.Add((e.Command.ArgumentsAsString.Substring(e.Command.ArgumentsAsList[0].Length + 1), e.Command.ChatMessage.DisplayName, temp));
                            SendMessage(String.Format("{0} успешно сделал заказ!", e.Command.ChatMessage.DisplayName));
                            OrdersUpdate(ListOrders);
                        }
                        else
                            SendMessage(String.Format("{0} у тебя недостаточно игрушек!", e.Command.ChatMessage.DisplayName));
                    }
                    else
                        SendMessage("Некорректное использование команды");
                }
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
    }

}
