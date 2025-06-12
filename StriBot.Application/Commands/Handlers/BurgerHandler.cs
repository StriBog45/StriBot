using System.Collections.Generic;
using System.Text;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Commands.Extensions;
using StriBot.Application.Commands.Models;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;
using StriBot.Application.Extensions;

namespace StriBot.Application.Commands.Handlers;

public class BurgerHandler
{
    private const int MaxBurgerSize = 3;

    private static readonly string[] ListStuffing =
    [
        "сосисками",
        "бабушкиной аджикой, ядреной такой",
        "ломтиком сыра",
        "плавленным сыром",
        "плавленным шоколадом",
        "соленым огурцом",
        "свежим огурцом",
        "маринованным огурцом",
        "черной икрой",
        "красной икрой",
        "майонезом",
        "тертым чесноком",
        "чесночным соусом",
        "чесноком кубиками",
        "горчицей",
        "кетчупом",
        "помидором",
        "варенной колбасой",
        "салями",
        "шпротами",
        "красной рыбой",
        "ветчиной",
        "котлетой",
        "яйцом",
        "тунцом",
        "баклажаном",
        "оливками",
        "хреном",
        "капустой",
        "петрушкой",
        "зеленью",
        "креветками",
        "шоколадным сыром",
        "творогом",
        "рыбным филе",
        "грибами",
        "хлебом"
    ];
    private static readonly string[] ListFoundation =
    [
        "горячем хлебушке",
        "горячей булочке",
        "горячем ломтике хлеба",
        "кусочке батона",
        "хрустящем и поджаренном ломтике хлеба",
        "черном хлебе",
        "лаваше"
    ];
        
    private static string BurgerCombiner()
    {
        var burgerSize = RandomHelper.Random.Next(1, MaxBurgerSize);
        var burgerBuilder = new StringBuilder("бутерброд с ");
        for(var i=0; i<burgerSize; i++)
        {
            if (i == 0)
                burgerBuilder.Append(RandomHelper.GetRandomOfArray(ListStuffing));
            else
                burgerBuilder.Append(" и " + RandomHelper.GetRandomOfArray(ListStuffing));
        }
        burgerBuilder.Append($" на {RandomHelper.GetRandomOfArray(ListFoundation)}");
        return burgerBuilder.ToString();
    }

    public static Dictionary<string, Command> CreateCommands()
        => new()
        {
            new Command("Бутерброд","Выдает бутерброд тебе или объекту",
                delegate (CommandInfo commandInfo)
                {
                    EventContainer.Message(
                        string.IsNullOrEmpty(commandInfo.ArgumentsAsString)
                            ? $"Несу {BurgerCombiner()} для {commandInfo.DisplayName}! TPFufun "
                            : $"Несу {BurgerCombiner()} для {commandInfo.ArgumentsAsString}! TPFufun ",
                        commandInfo.Platform);
                }, ["Объект"], CommandType.Interactive)
        };
}