using StriBot.Bots.Enums;
using StriBot.CustomData;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using System.Collections.Generic;

namespace StriBot.Commands
{
    public class RandomAnswerManager
    {
        private readonly CustomArray customArray;

        public RandomAnswerManager (CustomArray customArray)
        {
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
                delegate (CommandInfo commandInfo)
                {
                    if (string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                        SendMessage($"{commandInfo.DisplayName} бросил снежок и попал в себя!", commandInfo.Platform);
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
                        SendMessage($"{commandInfo.DisplayName} бросил снежок в {commandInfo.ArgumentsAsString} {snowResult}", commandInfo.Platform);
                    }
                }, new string[] { "Объект" }, CommandType.Interactive);

        private Command CreateRollCommand()
            => new Command("Roll", "Бросить Roll", 
                delegate (CommandInfo commandInfo) 
                { SendMessage($"{commandInfo.DisplayName} получает число: {RandomHelper.random.Next(0, 100)}", commandInfo.Platform); }, 
                new string[] { "Объект" }, CommandType.Interactive);

        private Command CreateLiftedCommand()
            => new Command("Подуть", "Дует на цель",
                delegate (CommandInfo commandInfo)
                {
                    if (string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                        SendMessage($"{commandInfo.DisplayName} подул на свой нос SMOrc !", commandInfo.Platform);
                    else
                    {
                        SendMessage(string.Format("{0} подул на {1}, поднимается юбка и мы обнаруживаем {2} {3}! PogChamp ",
                            commandInfo.DisplayName, commandInfo.ArgumentsAsString, customArray.GetUnderpantsType(), customArray.GetUnderpantsColor()), commandInfo.Platform);
                    }
                }, new string[] { "Цель" }, CommandType.Interactive);

        private Command CreateCompatibilityCommand()
            => new Command("Совместимость", "Проверяет вашу совместимость с объектом",
                delegate (CommandInfo commandInfo)
                {
                    if (string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                        SendMessage($"Совместимость {commandInfo.DisplayName} с собой составляет {RandomHelper.random.Next(0, 101)}%", commandInfo.Platform);
                    else
                        SendMessage($"{commandInfo.DisplayName} совместим с {commandInfo.ArgumentsAsString} на {RandomHelper.random.Next(0, 101)}%", commandInfo.Platform);
                }, new string[] { "Объект" }, CommandType.Interactive);

        private Command CreateBucketCommand()
            => new Command("Цветы", "Дарит букет цветов объекту",
                delegate (CommandInfo commandInfo)
                {
                    if (string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                        SendMessage($"{commandInfo.DisplayName} приобрел букет {customArray.GetBucket()} PepoFlower ", commandInfo.Platform);
                    else
                        SendMessage($"{commandInfo.DisplayName} дарит {commandInfo.ArgumentsAsString} букет {customArray.GetBucket()} PepoFlower ", commandInfo.Platform);
                }, new string[] { "Объект" }, CommandType.Interactive);

        private Command CreateLoveCommand()
            => new Command("Люблю", "Показывает насколько вы любите объект",
                delegate (CommandInfo commandInfo)
                {
                    if (string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                        SendMessage(string.Format("{0} любит себя на {1}% <3 ", commandInfo.DisplayName, RandomHelper.random.Next(0, 101)), commandInfo.Platform);
                    else
                        SendMessage(string.Format("{0} любит {1} на {2}% <3 ", commandInfo.DisplayName, commandInfo.ArgumentsAsString, RandomHelper.random.Next(0, 101)), commandInfo.Platform);
                }, new string[] { "Объект" }, CommandType.Interactive);

        private Command CreateDuelCommand()
            => new Command("Duel", "Вызывает объект на дуэль в доте 1х1",
                delegate (CommandInfo commandInfo)
                {
                    if (!string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                    {
                        var duelAccuraccy = RandomHelper.random.Next(0, 100);
                        string duelResult = customArray.GetDota2DuelResult();
                        SendMessage(string.Format("{0} вызывает {1} на битву 1х1 на {2}! Итог: {3}",
                            commandInfo.DisplayName, commandInfo.ArgumentsAsString, Heroes.GetRandomHero(), duelResult), commandInfo.Platform);
                    }
                    else
                        SendMessage(string.Format("В дуэли между {0} и {0} победил {0}!", commandInfo.DisplayName), commandInfo.Platform);
                }, new string[] { "Объект" }, CommandType.Interactive);

        private Command CreateIqCommand()
            => new Command("IQ", "Узнать IQ объекта или свой",
                delegate (CommandInfo commandInfo)
                {
                    if (string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                        SendMessage($"Ваш IQ: {RandomHelper.random.Next(1, 200)} SeemsGood ", commandInfo.Platform);
                    else
                        SendMessage($"IQ {commandInfo.ArgumentsAsString} составляет: {RandomHelper.random.Next(1, 200)}! SeemsGood ", commandInfo.Platform);
                }, new string[] { "Объект" }, CommandType.Interactive);

        private Command CreateMagic8Ball()
            => new Command("Шар", "Шар предсказаний, формулируйте вопрос для ответа \"да\" или \"нет\" ",
                delegate (CommandInfo commandInfo)
                {
                    SendMessage($"Шар говорит... {customArray.GetBallAnswer()}", commandInfo.Platform);
                }, new string[] { "Вопрос" }, CommandType.Interactive);

        private Command CommandCoin()
            => new Command("Монетка", "Орел или решка?",
                delegate (CommandInfo commandInfo)
                {
                    int coin = RandomHelper.random.Next(0, 101);
                    if (coin == 100)
                        SendMessage("Бросаю монетку... Ребро POGGERS ", commandInfo.Platform);
                    else
                        SendMessage($"Бросаю монетку... {(coin < 50 ? "Орел" : "Решка")}", commandInfo.Platform);
                }, CommandType.Interactive);

        private void SendMessage(string message, Platform platform)
            => GlobalEventContainer.Message(message, platform);
    }
}