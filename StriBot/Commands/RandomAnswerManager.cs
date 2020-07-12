using StriBot.CustomData;
using StriBot.TwitchBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace StriBot.Commands
{
    public class RandomAnswerManager
    {
        private readonly ITwitchBot twitchBot;
        private readonly CustomArray customArray;

        public RandomAnswerManager (ITwitchBot twitchBot, CustomArray customArray)
        {
            this.twitchBot = twitchBot;
            this.customArray = customArray;
        }

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>()
            {
                CreateSnowCommand(),
                CreateRollCommand(),
                CreateLiftedCommand(),
                CreateCompatibilityCommand(),
                CreateBucketCommand(),
                CreateLoveCommand(),
                CreateDuelCommand(),
                CreateIqCommand(),
                CreateMagic8Ball(),
                CommandCoin()
            };

        private Command CreateSnowCommand()
            => new Command("Снежок", "Бросает снежок в объект",
                delegate (OnChatCommandReceivedArgs e)
                {
                    if (string.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} бросил снежок и попал в себя!");
                    else
                    {
                        var accuracy = RandomHelper.random.Next(0, 100);
                        string snowResult = string.Empty;
                        if (accuracy < 10)
                            snowResult = "и... промазал";
                        if (accuracy >= 10 && accuracy <= 20)
                            snowResult = "но цель уклонилась KEKW ";
                        if (accuracy > 30)
                            snowResult = $"и попал {customArray.GetHited()}";
                        twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} бросил снежок в {e.Command.ArgumentsAsString} {snowResult}");
                    }
                }, new string[] { "Объект" }, CommandType.Interactive);

        private Command CreateRollCommand()
            => new Command("Roll", "Бросить Roll", 
                delegate (OnChatCommandReceivedArgs e) 
                { twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} получает число: {RandomHelper.random.Next(0, 100)}"); }, 
                new string[] { "Объект" }, CommandType.Interactive);

        private Command CreateLiftedCommand()
            => new Command("Подуть", "Дует на цель",
                delegate (OnChatCommandReceivedArgs e)
                {
                    if (string.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} подул на свой нос SMOrc !");
                    else
                    {
                        twitchBot.SendMessage(string.Format("{0} подул на {1}, поднимается юбка и мы обнаруживаем {2} {3}! PogChamp ",
                            e.Command.ChatMessage.DisplayName, e.Command.ArgumentsAsString, customArray.GetUnderpantsType(), customArray.GetUnderpantsColor()));
                    }
                }, new string[] { "Цель" }, CommandType.Interactive);

        private Command CreateCompatibilityCommand()
            => new Command("Совместимость", "Проверяет вашу совместимость с объектом",
                delegate (OnChatCommandReceivedArgs e)
                {
                    if (string.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        twitchBot.SendMessage($"Совместимость {e.Command.ChatMessage.DisplayName} с собой составляет {RandomHelper.random.Next(0, 101)}%");
                    else
                        twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} совместим с {e.Command.ArgumentsAsString} на {RandomHelper.random.Next(0, 101)}%");
                }, new string[] { "Объект" }, CommandType.Interactive);

        private Command CreateBucketCommand()
            => new Command("Цветы", "Дарит букет цветов объекту",
                delegate (OnChatCommandReceivedArgs e)
                {
                    if (string.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} приобрел букет {customArray.GetBucket()} PepoFlower ");
                    else
                        twitchBot.SendMessage($"{e.Command.ChatMessage.DisplayName} дарит {e.Command.ArgumentsAsString} букет {customArray.GetBucket()} PepoFlower ");
                }, new string[] { "Объект" }, CommandType.Interactive);

        private Command CreateLoveCommand()
            => new Command("Люблю", "Показывает насколько вы любите объект",
                delegate (OnChatCommandReceivedArgs e)
                {
                    if (string.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        twitchBot.SendMessage(string.Format("{0} любит себя на {1}% <3 ", e.Command.ChatMessage.DisplayName, RandomHelper.random.Next(0, 101)));
                    else
                        twitchBot.SendMessage(string.Format("{0} любит {1} на {2}% <3 ", e.Command.ChatMessage.DisplayName, e.Command.ArgumentsAsString, RandomHelper.random.Next(0, 101)));
                }, new string[] { "Объект" }, CommandType.Interactive);

        private Command CreateDuelCommand()
            => new Command("Duel", "Вызывает объект на дуэль в доте 1х1",
                delegate (OnChatCommandReceivedArgs e)
                {
                    if (!string.IsNullOrEmpty(e.Command.ArgumentsAsString))
                    {
                        var duelAccuraccy = RandomHelper.random.Next(0, 100);
                        string duelResult = customArray.GetDota2DuelResult();
                        twitchBot.SendMessage(string.Format("{0} вызывает {1} на битву 1х1 на {2}! Итог: {3}",
                            e.Command.ChatMessage.DisplayName, e.Command.ArgumentsAsString, Heroes.GetRandomHero(), duelResult));
                    }
                    else
                        twitchBot.SendMessage(string.Format("В дуэли между {0} и {0} победил {0}!", e.Command.ChatMessage.DisplayName));
                }, new string[] { "Объект" }, CommandType.Interactive);

        private Command CreateIqCommand()
            => new Command("IQ", "Узнать IQ объекта или свой",
                delegate (OnChatCommandReceivedArgs e)
                {
                    if (string.IsNullOrEmpty(e.Command.ArgumentsAsString))
                        twitchBot.SendMessage($"Ваш IQ: {RandomHelper.random.Next(1, 200)} SeemsGood ");
                    else
                        twitchBot.SendMessage($"IQ {e.Command.ArgumentsAsString} составляет: {RandomHelper.random.Next(1, 200)}! SeemsGood ");
                }, new string[] { "Объект" }, CommandType.Interactive);

        private Command CreateMagic8Ball()
            => new Command("Шар", "Шар предсказаний, формулируйте вопрос для ответа \"да\" или \"нет\" ",
                delegate (OnChatCommandReceivedArgs e)
                {
                    twitchBot.SendMessage($"Шар говорит... {customArray.GetBallAnswer()}");
                }, new string[] { "Вопрос" }, CommandType.Interactive);

        private Command CommandCoin()
            => new Command("Монетка", "Орел или решка?",
                delegate (OnChatCommandReceivedArgs e)
                {
                    int coin = RandomHelper.random.Next(0, 101);
                    if (coin == 100)
                        twitchBot.SendMessage("Бросаю монетку... Ребро POGGERS ");
                    else
                        twitchBot.SendMessage($"Бросаю монетку... {(coin < 50 ? "Орел" : "Решка")}");
                }, CommandType.Interactive);
    }
}