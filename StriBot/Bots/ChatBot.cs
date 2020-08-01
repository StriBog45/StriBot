using DryIoc;
using StriBot.Bots.Enums;
using StriBot.Commands;
using StriBot.Commands.CommonFunctions;
using StriBot.CustomData;
using StriBot.DryIoc;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using StriBot.Language;
using StriBot.Speakers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StriBot.Bots
{
    public class ChatBot
    {
        public Dictionary<string, Command> Commands { get; set; }
        public CollectionHelper Bosses { get; set; }
        public int Deaths { get; set; } = 0;
        public string TextReminder { get; set; } = string.Empty;
        public Dictionary<string, (int, int)> UsersBetted { get; set; }

        private Action BossUpdate;
        private Action DeathUpdate;
        private CustomArray customArray;
        private string[] BettingOptions;
        private bool betsProcessing;
        private int betsTimer;
        private double betsCoefficient;
        private int timer;
        private int toysForSub = 30;
        private readonly Currency currency;
        private readonly CurrencyBaseManager _currencyBaseManager;
        private readonly HalberdManager _halberdManager;
        private readonly DuelManager _duelManager;
        private readonly Speaker _speaker;
        private readonly TwitchBot _twitchBot;

        public ChatBot(Currency currency, Speaker speaker, CustomArray customArray, TwitchBot twitchBot, DuelManager duelManager, HalberdManager halberdManager, CurrencyBaseManager currencyBaseManager)
        {
            this.customArray = customArray;
            this.currency = currency;
            this._speaker = speaker;
            this._twitchBot = twitchBot;
            _halberdManager = halberdManager;
            _currencyBaseManager = currencyBaseManager;
            _duelManager = duelManager;

            UsersBetted = new Dictionary<string, (int, int)>();
            Bosses = new CollectionHelper();

            GlobalEventContainer.CommandReceived += OnChatCommandReceived;
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
                _currencyBaseManager.DistributionMoney(1, 5, Enums.Platform.Twitch);
            if (timer == 15)
                SendMessage("Если увидел крутой момент, запечатли это! Сделай клип! striboF ");
            if (timer == 30)
                SendMessage("У стримера все под контролем! striboPled ");
            if (timer == 45)
                SendMessage("Спасибо за вашу поддержку! HolidaySanta ");

            if (timer % 10 == 0 && !string.IsNullOrEmpty(TextReminder))
                SendMessage("Напоминание: " + TextReminder);

            if (timer == 60)
                timer = 0;

            _duelManager.Tick();
            _halberdManager.Tick();
        }

        public void Connect(Platform[] platforms)
        {
            foreach (var platform in platforms)
            {
                switch (platform)
                {
                    case Platform.Twitch:
                        _twitchBot.Connect();
                        break;
                }
            }
        }

        internal void Reconnect(Platform[] platforms)
        {
            foreach (var platform in platforms)
            {
                switch (platform)
                {
                    case Platform.Twitch:
                        _twitchBot.Reconnect();
                        break;
                }
            }
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

            SendMessage(string.Format("Время ставок! Коэффициент {0}. Для участия необходимо написать !ставка [номер ставки] [сколько ставите]", betsCoefficient));
            StringBuilder messageBuilder = new StringBuilder(string.Format("{0}: ", options[0]));
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

        public void SetConstructorSettings(Action bossUpdate, Action deathUpdate)
        {
            BossUpdate = bossUpdate;
            DeathUpdate = deathUpdate;
        }

        public void CreateCommands()
        {
            var container = GlobalContainer.Default;
            var managerMMR = container.Resolve<MMRManager>();
            var readyMadePhrases = container.Resolve<ReadyMadePhrases>();
            var orderManager = container.Resolve<OrderManager>();
            var linkManager = container.Resolve<LinkManager>();
            var randomAnswerManager = container.Resolve<RandomAnswerManager>();
            var burgerManager = container.Resolve<BurgerManager>();
            

            Commands = new Dictionary<string, Command>()
            {
                linkManager.CreateCommands(),
                managerMMR.CreateCommands(),
                randomAnswerManager.CreateCommands(),
                burgerManager.CreateCommands(),

                //{ "uptime", new Command("Uptime","Длительность текущей трансляции",
                //delegate (CommandInfo e) {
                //    SendMessage( $"Трансляция длится: { GetUptime()}"); }, CommandType.Info)},

                #region Интерактив
                { "размерг", new Command("РазмерГ","Узнать размер вашей груди",
                delegate (CommandInfo e) {
                    int size = RandomHelper.random.Next(0,7);

                    if(size == 0)
                        SendMessage(string.Format("0 размер... Извините, {0}, а что мерить? KEKW ", e.DisplayName));
                    if(size == 1)
                        SendMessage(string.Format("1 размер... Не переживай {0}, ещё вырастут striboCry ", e.DisplayName));
                    if(size == 2)
                        SendMessage(string.Format("2 размер... {0}, ваши груди отлично помещаются в ладошки! billyReady ", e.DisplayName));
                    if(size == 3)
                        SendMessage(string.Format("3 размер... Идеально... Kreygasm , {0} оставьте мне ваш номерок", e.DisplayName));
                    if(size == 4)
                        SendMessage(string.Format("4 размер... Внимание мужчин к {0} обеспечено striboPled ", e.DisplayName));
                    if(size == 5)
                        SendMessage(string.Format("5 размер... В грудях {0} можно утонуть счастливым Kreygasm", e.DisplayName));
                    if(size == 6)
                        SendMessage(string.Format("6 размер... В ваших руках... Кхм, на грудной клетке {0} две убийственные груши", e.DisplayName));
                }, CommandType.Interactive)},
                { "размерп", new Command("РазмерП","Узнать размер вашего писюна",
                delegate (CommandInfo e) {
                    int size = RandomHelper.random.Next(10,21);

                    if(size < 13)
                        SendMessage(string.Format("{0} сантиметров... {1}, не переживай, размер не главное! ", e.DisplayName,size));
                    else if(size == 13)
                        SendMessage(string.Format("13 сантиметров... {0}, поздравляю, у вас стандарт!  striboF ", e.DisplayName));
                    else if(size == 20)
                        SendMessage(string.Format("20 сантиметров... {0}, вы можете завернуть свой шланг обратно monkaS ", e.DisplayName));
                    else
                        SendMessage(string.Format("{0} сантиметров... {1}, ваша девушка... или мужчина, будет в восторге! striboTea ", e.DisplayName,size));
                }, CommandType.Interactive)},
                { "смерть", new Command("Смерть", "Добавляет смерть", Role.Moderator,
                delegate (CommandInfo e) {
                    Deaths++;
                    DeathUpdate();
                    SendMessage(string.Format("Смертей: {0}", Deaths));
                    SendMessage("▬▬▬▬▬▬▬▬▬▬ஜ۩۞۩ஜ▬▬▬▬▬▬▬▬▬ ……………..............……...Ｙ Ｏ Ｕ Ｄ Ｉ Ｅ Ｄ…….……….........…..… ▬▬▬▬▬▬▬▬▬▬ஜ۩۞۩ஜ▬▬▬▬▬▬▬▬▬"); }, CommandType.Interactive)},
                { "смертей", new Command("Смертей", "Показывает количество смертей",
                delegate (CommandInfo e) {
                    SendMessage(string.Format("Смертей: {0}",Deaths)); }, CommandType.Interactive)},
                { "босс", new Command("Босс", "Добавляет босса", Role.Moderator,
                delegate (CommandInfo e) {
                    Bosses.Add(e.ArgumentsAsString);
                    BossUpdate();
                }, new string[] {"Имя босса"}, CommandType.Interactive )},
                { "напомнить", new Command("Напомнить", "Создает напоминалку. При использовании без указания текста, напоминание будет удалено", Role.Moderator,
                delegate (CommandInfo e) {
                    TextReminder = e.ArgumentsAsString;
                    if(TextReminder.Length > 0)
                        SendMessage($"Напоминание: \"{e.ArgumentsAsString}\" создано");
                    else
                        SendMessage("Напоминание удалено");
                }, new string[] {"текст"}, CommandType.Interactive )},
                { "боссы", new Command("Боссы", "Список убитых боссов!",
                delegate (CommandInfo e) {
                    if(Bosses.Count > 0)
                        SendMessage(Bosses.ToString());
                    else
                        SendMessage("Боссов нет"); }, CommandType.Interactive)},

                #endregion

                orderManager.CreateCommands(),
                _currencyBaseManager.CreateCommands(),
                _duelManager.CreateCommands(),
                _halberdManager.CreateCommands(),

                #region DateBase
                { "ставка", new Command("Ставка", "Сделать ставку",
                delegate (CommandInfo e) {
                    if(betsProcessing)
                    {
                        int numberOfBets = 0;
                        int amountOfBets = 0;
                        if (e.ArgumentsAsList.Count == 2 && Int32.TryParse(e.ArgumentsAsList[0], out numberOfBets) && Int32.TryParse(e.ArgumentsAsList[1],out amountOfBets)
                        && numberOfBets < BettingOptions.Length && amountOfBets > 0)
                        {
                            if (DataBase.CheckMoney(e.DisplayName) < amountOfBets)
                                SendMessage($"{e.DisplayName} у тебя недостаточно {currency.GenitiveMultiple} для такой ставки!");
                            else
                            {
                                if (!UsersBetted.ContainsKey(e.DisplayName))
                                {
                                    UsersBetted.Add(e.DisplayName, (numberOfBets,amountOfBets));
                                    SendMessage($"{e.DisplayName} успешно сделал ставку!");
                                }
                                else
                                    SendMessage($"{e.DisplayName} уже сделал ставку!");
                            }
                        }
                        else
                            SendMessage($"{e.DisplayName} вы неправильно указали ставку");
                    }
                    else
                        SendMessage("В данный момент ставить нельзя!");
                },
                new string[] {"на что", "сколько"}, CommandType.Interactive )},
                #endregion
            };
        }

        private void OnChatCommandReceived(CommandInfo commandInfo)
        {
            if (Commands.ContainsKey(commandInfo.CommandText))
            {
                if (IsAccessAllowed(Commands[commandInfo.CommandText].Requires, commandInfo)
                    && _halberdManager.CanSendMessage(commandInfo))
                    Commands[commandInfo.CommandText].Action(commandInfo);
            }
        }

        private bool IsAccessAllowed(Role role, CommandInfo commandInfo)
        {
            var result = true;

            switch (role)
            {
                case Role.Subscriber:
                    result = commandInfo.IsSubscriber.HasValue && commandInfo.IsSubscriber.Value;
                    break;
                case Role.Moderator:
                    result = (commandInfo.IsModerator.HasValue && commandInfo.IsModerator.Value) || (commandInfo.IsBroadcaster.HasValue && commandInfo.IsBroadcaster.Value);
                    break;
                case Role.Broadcaster:
                    result = commandInfo.IsBroadcaster.HasValue && commandInfo.IsBroadcaster.Value; 
                    break;
            }

            return result;
        }

        private void SendMessage(string text)
        {
            GlobalEventContainer.Message(text, Platform.Twitch);
        }
    }
}