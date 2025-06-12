using System.Collections.Generic;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Commands.Extensions;
using StriBot.Application.Commands.Models;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;
using StriBot.Application.Extensions;
using StriBot.Application.Localization;
using StriBot.Application.Platforms.Enums;

namespace StriBot.Application.Commands.Handlers;

public class RandomAnswerHandler(AnswerOptions customArray)
{
    public Dictionary<string, Command> CreateCommands()
        => new()
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
        => new("Снежок", "Бросает снежок в объект",
            delegate (CommandInfo commandInfo)
            {
                if (string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                    SendMessage($"{commandInfo.DisplayName} бросил снежок и попал в себя!", commandInfo.Platform);
                else
                {
                    var accuracy = RandomHelper.Random.Next(0, 100);
                    var snowResult = string.Empty;
                    if (accuracy < 10)
                        snowResult = "и... промазал";
                    if (accuracy is >= 10 and <= 20)
                        snowResult = "но цель уклонилась KEKW ";
                    if (accuracy > 30)
                        snowResult = $"и попал {customArray.GetHited()}";
                    SendMessage($"{commandInfo.DisplayName} бросил снежок в {commandInfo.ArgumentsAsString} {snowResult}", commandInfo.Platform);
                }
            }, ["Объект"], CommandType.Interactive);

    private static Command CreateRollCommand()
        => new("Roll", "Бросить Roll", 
            delegate (CommandInfo commandInfo) 
            { SendMessage($"{commandInfo.DisplayName} получает число: {RandomHelper.Random.Next(0, 100)}", commandInfo.Platform); },
            ["Объект"], CommandType.Interactive);

    private Command CreateLiftedCommand()
        => new("Подуть", "Дует на цель",
            delegate (CommandInfo commandInfo)
            {
                if (string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                    SendMessage($"{commandInfo.DisplayName} подул на свой нос SMOrc !", commandInfo.Platform);
                else
                {
                    SendMessage(string.Format("{0} подул на {1}, поднимается юбка и мы обнаруживаем {2} {3}! PogChamp ",
                        commandInfo.DisplayName, commandInfo.ArgumentsAsString, customArray.GetUnderpantsType(), customArray.GetUnderpantsColor()), commandInfo.Platform);
                }
            }, ["Цель"], CommandType.Interactive);

    private Command CreateCompatibilityCommand()
        => new("Совместимость", "Проверяет вашу совместимость с объектом",
            delegate (CommandInfo commandInfo)
            {
                SendMessage(
                    string.IsNullOrEmpty(commandInfo.ArgumentsAsString)
                        ? $"Совместимость {commandInfo.DisplayName} с собой составляет {RandomHelper.Random.Next(0, 101)}%"
                        : $"{commandInfo.DisplayName} совместим с {commandInfo.ArgumentsAsString} на {RandomHelper.Random.Next(0, 101)}%",
                    commandInfo.Platform);
            }, ["Объект"], CommandType.Interactive);

    private Command CreateBucketCommand()
        => new("Цветы", "Дарит букет цветов объекту",
            delegate (CommandInfo commandInfo)
            {
                SendMessage(
                    string.IsNullOrEmpty(commandInfo.ArgumentsAsString)
                        ? $"{commandInfo.DisplayName} приобрел букет {customArray.GetBucket()} PepoFlower "
                        : $"{commandInfo.DisplayName} дарит {commandInfo.ArgumentsAsString} букет {customArray.GetBucket()} PepoFlower ",
                    commandInfo.Platform);
            }, ["Объект"], CommandType.Interactive);

    private static Command CreateLoveCommand()
        => new("Люблю", "Показывает насколько вы любите объект",
            delegate (CommandInfo commandInfo)
            {
                SendMessage(
                    string.IsNullOrEmpty(commandInfo.ArgumentsAsString)
                        ? $"{commandInfo.DisplayName} любит себя на {RandomHelper.Random.Next(0, 101)}% <3 "
                        : $"{commandInfo.DisplayName} любит {commandInfo.ArgumentsAsString} на {RandomHelper.Random.Next(0, 101)}% <3 ",
                    commandInfo.Platform);
            }, ["Объект"], CommandType.Interactive);

    private Command CreateDuelCommand()
        => new("Duel", "Вызывает объект на дуэль в доте 1х1",
            delegate (CommandInfo commandInfo)
            {
                if (!string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                {
                    var duelResult = customArray.GetDota2DuelResult();
                    SendMessage($"{commandInfo.DisplayName} вызывает {commandInfo.ArgumentsAsString} на битву 1х1 на {Heroes.GetRandomHero()}! Итог: {duelResult}", commandInfo.Platform);
                }
                else
                    SendMessage(string.Format("В дуэли между {0} и {0} победил {0}!", commandInfo.DisplayName), commandInfo.Platform);
            }, ["Объект"], CommandType.Interactive);

    private static Command CreateIqCommand()
        => new("IQ", "Узнать IQ объекта или свой",
            delegate (CommandInfo commandInfo)
            {
                SendMessage(
                    string.IsNullOrEmpty(commandInfo.ArgumentsAsString)
                        ? $"Ваш IQ: {RandomHelper.Random.Next(1, 200)} SeemsGood "
                        : $"IQ {commandInfo.ArgumentsAsString} составляет: {RandomHelper.Random.Next(1, 200)}! SeemsGood ",
                    commandInfo.Platform);
            }, ["Объект"], CommandType.Interactive);

    private Command CreateMagic8Ball()
        => new("Шар", "Шар предсказаний, формулируйте вопрос для ответа \"да\" или \"нет\" ",
            delegate (CommandInfo commandInfo)
            {
                SendMessage($"Шар говорит... {customArray.GetBallAnswer()}", commandInfo.Platform);
            }, ["Вопрос"], CommandType.Interactive);

    private static Command CommandCoin()
        => new("Монетка", "Орел или решка?",
            delegate (CommandInfo commandInfo)
            {
                var coin = RandomHelper.Random.Next(0, 101);
                SendMessage(
                    coin == 100
                        ? "Бросаю монетку... Ребро POGGERS "
                        : $"Бросаю монетку... {(coin < 50 ? "Орел" : "Решка")}", commandInfo.Platform);
            }, CommandType.Interactive);

    private static Command CreateCommandBreastSize()
        => new("РазмерГ", "Узнать размер вашей груди",
            delegate (CommandInfo commandInfo) {
                var breastSize = RandomHelper.Random.Next(0, 7);
                var result = string.Empty;

                switch (breastSize)
                {
                    case 0:
                        result = $"0 размер... Извините, {commandInfo.DisplayName}, а что мерить? KEKW ";
                        break;
                    case 1:
                        result = $"1 размер... Не переживай {commandInfo.DisplayName}, ещё вырастут striboCry ";
                        break;
                    case 2:
                        result = $"2 размер... {commandInfo.DisplayName}, ваши груди отлично помещаются в ладошки! billyReady ";
                        break;
                    case 3:
                        result = $"3 размер... Идеально... Kreygasm , {commandInfo.DisplayName} оставьте мне ваш номерок";
                        break;
                    case 4:
                        result = $"4 размер... Внимание мужчин к {commandInfo.DisplayName} обеспечено striboPled ";
                        break;
                    case 5:
                        result = $"5 размер... В грудях {commandInfo.DisplayName} можно утонуть счастливым Kreygasm";
                        break;
                    case 6:
                        result = $"6 размер... В ваших руках... Кхм, на грудной клетке {commandInfo.DisplayName} две убийственные груши";
                        break;
                }

                SendMessage(result, commandInfo.Platform);
            }, CommandType.Interactive);

    private static Command CreateCommandPenisSize()
        => new("РазмерП", "Узнать размер вашего писюна",
            delegate (CommandInfo commandInfo)
            {
                var penisSize = RandomHelper.Random.Next(10, 21);
                string result;

                if (penisSize < 13)
                    result = $"{commandInfo.DisplayName} сантиметров... {penisSize}, не переживай, размер не главное! ";
                else
                    result = penisSize switch
                    {
                        13 => $"13 сантиметров... {commandInfo.DisplayName}, поздравляю, у вас стандарт!  striboF ",
                        20 =>
                            $"20 сантиметров... {commandInfo.DisplayName}, вы можете завернуть свой шланг обратно monkaS ",
                        _ =>
                            $"{commandInfo.DisplayName} сантиметров... {penisSize}, ваша девушка... или мужчина, будет в восторге! striboTea "
                    };

                SendMessage(result, commandInfo.Platform);
            }, CommandType.Interactive);

    private static void SendMessage(string message, Platform platform)
        => EventContainer.Message(message, platform);
}