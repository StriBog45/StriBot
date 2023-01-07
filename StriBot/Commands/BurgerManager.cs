using System.Collections.Generic;
using System.Text;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;
using StriBot.Application.Extensions;
using StriBot.Commands.Extensions;
using StriBot.Commands.Models;

namespace StriBot.Commands
{
    public class BurgerManager
    {
        private static readonly int _maxBurgerSize = 3;
        private static readonly string[] ListStuffing =  {
            "сосисками",
            "бабушкиной аджикой, едренной такой",
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
            "горчицой",
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
        };
        private static readonly string[] ListFoundation = {
            "горячем хлебушке",
            "горячей булочке",
            "горячем ломтике хлеба",
            "кусочке батона",
            "хрустящем и поджаренном ломтике хлеба",
            "черном хлебе",
            "лаваше"
        };
        
        private static string BurgerCombiner()
        {
            var burgerSize = RandomHelper.Random.Next(1, _maxBurgerSize);
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

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>
            {
                new Command("Бутерброд","Выдает бутерброд тебе или объекту",
                delegate (CommandInfo commandInfo)
                {
                        if(string.IsNullOrEmpty( commandInfo.ArgumentsAsString))
                            EventContainer.Message(string.Format("Несу {0} для {1}! HahaCat ", BurgerCombiner(), commandInfo.DisplayName), commandInfo.Platform);
                        else
                            EventContainer.Message(string.Format("Несу {0} для {1}! HahaCat ", BurgerCombiner(), commandInfo.ArgumentsAsString), commandInfo.Platform);
                }, new[] {"Объект"}, CommandType.Interactive)
            };
    }
}