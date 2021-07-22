using StriBot.Bots.Enums;
using StriBot.Commands.Enums;
using StriBot.Commands.Extensions;
using StriBot.Commands.Models;
using StriBot.CustomData;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using System.Collections.Generic;

namespace StriBot.Commands
{
    public class RandomAnswerManager
    {
        private readonly AnswerOptions _customArray;

        public RandomAnswerManager (AnswerOptions customArray)
        {
            _customArray = customArray;
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
                CommandCoin(),
                CreateCommandBreastSize(),
                CreateCommandPenisSize()
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
                        var snowResult = string.Empty;
                        if (accuracy < 10)
                            snowResult = "и... промазал";
                        if (accuracy >= 10 && accuracy <= 20)
                            snowResult = "но цель уклонилась KEKW ";
                        if (accuracy > 30)
                            snowResult = $"и попал {_customArray.GetHited()}";
                        SendMessage($"{commandInfo.DisplayName} бросил снежок в {commandInfo.ArgumentsAsString} {snowResult}", commandInfo.Platform);
                    }
                }, new[] { "Объект" }, CommandType.Interactive);

        private Command CreateRollCommand()
            => new Command("Roll", "Бросить Roll", 
                delegate (CommandInfo commandInfo) 
                { SendMessage($"{commandInfo.DisplayName} получает число: {RandomHelper.random.Next(0, 100)}", commandInfo.Platform); }, 
                new[] { "Объект" }, CommandType.Interactive);

        private Command CreateLiftedCommand()
            => new Command("Подуть", "Дует на цель",
                delegate (CommandInfo commandInfo)
                {
                    if (string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                        SendMessage($"{commandInfo.DisplayName} подул на свой нос SMOrc !", commandInfo.Platform);
                    else
                    {
                        SendMessage(string.Format("{0} подул на {1}, поднимается юбка и мы обнаруживаем {2} {3}! PogChamp ",
                            commandInfo.DisplayName, commandInfo.ArgumentsAsString, _customArray.GetUnderpantsType(), _customArray.GetUnderpantsColor()), commandInfo.Platform);
                    }
                }, new[] { "Цель" }, CommandType.Interactive);

        private Command CreateCompatibilityCommand()
            => new Command("Совместимость", "Проверяет вашу совместимость с объектом",
                delegate (CommandInfo commandInfo)
                {
                    SendMessage(
                        string.IsNullOrEmpty(commandInfo.ArgumentsAsString)
                            ? $"Совместимость {commandInfo.DisplayName} с собой составляет {RandomHelper.random.Next(0, 101)}%"
                            : $"{commandInfo.DisplayName} совместим с {commandInfo.ArgumentsAsString} на {RandomHelper.random.Next(0, 101)}%",
                        commandInfo.Platform);
                }, new[] { "Объект" }, CommandType.Interactive);

        private Command CreateBucketCommand()
            => new Command("Цветы", "Дарит букет цветов объекту",
                delegate (CommandInfo commandInfo)
                {
                    SendMessage(
                        string.IsNullOrEmpty(commandInfo.ArgumentsAsString)
                            ? $"{commandInfo.DisplayName} приобрел букет {_customArray.GetBucket()} PepoFlower "
                            : $"{commandInfo.DisplayName} дарит {commandInfo.ArgumentsAsString} букет {_customArray.GetBucket()} PepoFlower ",
                        commandInfo.Platform);
                }, new[] { "Объект" }, CommandType.Interactive);

        private Command CreateLoveCommand()
            => new Command("Люблю", "Показывает насколько вы любите объект",
                delegate (CommandInfo commandInfo)
                {
                    if (string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                        SendMessage(string.Format("{0} любит себя на {1}% <3 ", commandInfo.DisplayName, RandomHelper.random.Next(0, 101)), commandInfo.Platform);
                    else
                        SendMessage(string.Format("{0} любит {1} на {2}% <3 ", commandInfo.DisplayName, commandInfo.ArgumentsAsString, RandomHelper.random.Next(0, 101)), commandInfo.Platform);
                }, new[] { "Объект" }, CommandType.Interactive);

        private Command CreateDuelCommand()
            => new Command("Duel", "Вызывает объект на дуэль в доте 1х1",
                delegate (CommandInfo commandInfo)
                {
                    if (!string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                    {
                        var duelResult = _customArray.GetDota2DuelResult();
                        SendMessage(string.Format("{0} вызывает {1} на битву 1х1 на {2}! Итог: {3}",
                            commandInfo.DisplayName, commandInfo.ArgumentsAsString, Heroes.GetRandomHero(), duelResult), commandInfo.Platform);
                    }
                    else
                        SendMessage(string.Format("В дуэли между {0} и {0} победил {0}!", commandInfo.DisplayName), commandInfo.Platform);
                }, new[] { "Объект" }, CommandType.Interactive);

        private Command CreateIqCommand()
            => new Command("IQ", "Узнать IQ объекта или свой",
                delegate (CommandInfo commandInfo)
                {
                    SendMessage(
                        string.IsNullOrEmpty(commandInfo.ArgumentsAsString)
                            ? $"Ваш IQ: {RandomHelper.random.Next(1, 200)} SeemsGood "
                            : $"IQ {commandInfo.ArgumentsAsString} составляет: {RandomHelper.random.Next(1, 200)}! SeemsGood ",
                        commandInfo.Platform);
                }, new[] { "Объект" }, CommandType.Interactive);

        private Command CreateMagic8Ball()
            => new Command("Шар", "Шар предсказаний, формулируйте вопрос для ответа \"да\" или \"нет\" ",
                delegate (CommandInfo commandInfo)
                {
                    SendMessage($"Шар говорит... {_customArray.GetBallAnswer()}", commandInfo.Platform);
                }, new[] { "Вопрос" }, CommandType.Interactive);

        private Command CommandCoin()
            => new Command("Монетка", "Орел или решка?",
                delegate (CommandInfo commandInfo)
                {
                    var coin = RandomHelper.random.Next(0, 101);
                    SendMessage(
                        coin == 100
                            ? "Бросаю монетку... Ребро POGGERS "
                            : $"Бросаю монетку... {(coin < 50 ? "Орел" : "Решка")}", commandInfo.Platform);
                }, CommandType.Interactive);

        private Command CreateCommandBreastSize()
            => new Command("РазмерГ", "Узнать размер вашей груди",
                delegate (CommandInfo commandInfo) {
                    var breastSize = RandomHelper.random.Next(0, 7);
                    var result = string.Empty;

                    switch (breastSize)
                    {
                        case 0:
                            result = string.Format("0 размер... Извините, {0}, а что мерить? KEKW ", commandInfo.DisplayName);
                            break;
                        case 1:
                            result = string.Format("1 размер... Не переживай {0}, ещё вырастут striboCry ", commandInfo.DisplayName);
                            break;
                        case 2:
                            result = string.Format("2 размер... {0}, ваши груди отлично помещаются в ладошки! billyReady ", commandInfo.DisplayName);
                            break;
                        case 3:
                            result = string.Format("3 размер... Идеально... Kreygasm , {0} оставьте мне ваш номерок", commandInfo.DisplayName);
                            break;
                        case 4:
                            result = string.Format("4 размер... Внимание мужчин к {0} обеспечено striboPled ", commandInfo.DisplayName);
                            break;
                        case 5:
                            result = string.Format("5 размер... В грудях {0} можно утонуть счастливым Kreygasm", commandInfo.DisplayName);
                            break;
                        case 6:
                            result = string.Format("6 размер... В ваших руках... Кхм, на грудной клетке {0} две убийственные груши", commandInfo.DisplayName);
                            break;
                    }

                    SendMessage(result, commandInfo.Platform);
                }, CommandType.Interactive);

        private Command CreateCommandPenisSize()
            => new Command("РазмерП", "Узнать размер вашего писюна",
                delegate (CommandInfo commandInfo)
                {
                    var penisSize = RandomHelper.random.Next(10, 21);
                    var result = string.Empty;

                    if (penisSize < 13)
                        result = string.Format("{0} сантиметров... {1}, не переживай, размер не главное! ", commandInfo.DisplayName, penisSize);
                    else switch (penisSize)
                    {
                        case 13:
                            result = string.Format("13 сантиметров... {0}, поздравляю, у вас стандарт!  striboF ", commandInfo.DisplayName);
                            break;
                        case 20:
                            result = string.Format("20 сантиметров... {0}, вы можете завернуть свой шланг обратно monkaS ", commandInfo.DisplayName);
                            break;
                        default:
                            result = string.Format("{0} сантиметров... {1}, ваша девушка... или мужчина, будет в восторге! striboTea ", commandInfo.DisplayName, penisSize);
                            break;
                    }

                    SendMessage(result, commandInfo.Platform);
                }, CommandType.Interactive);

        private void SendMessage(string message, Platform platform)
            => GlobalEventContainer.Message(message, platform);
    }
}