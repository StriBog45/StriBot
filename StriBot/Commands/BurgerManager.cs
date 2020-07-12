using StriBot.TwitchBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace StriBot.Commands
{
    public class BurgerManager
    {
        private ITwitchBot twitchBot;

        public BurgerManager(ITwitchBot twitchBot)
        {
            this.twitchBot = twitchBot;
        }

        static int MAX_BURGER_SIZE = 3;
        static string[] ListStuffing =  {
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

        static string[] ListFoundation = {
            "горячем хлебушке",
            "горячей булочке",
            "горячем ломтике хлеба",
            "кусочке батона",
            "хрустящем и поджаренном ломтике хлеба",
            "черном хлебе",
            "лаваше"
        };
        
        public static string BurgerCombiner()
        {
            int burgerSize = RandomHelper.random.Next(1, MAX_BURGER_SIZE);
            StringBuilder burgerBuilder = new StringBuilder("бутерброд с ");
            for(int i=0; i<burgerSize; i++)
            {
                if (i == 0)
                    burgerBuilder.Append(RandomHelper.GetRandomOfArray(ListStuffing));
                else
                    burgerBuilder.Append(" и " + RandomHelper.GetRandomOfArray(ListStuffing));
            }
            burgerBuilder.Append(String.Format(" на {0}", RandomHelper.GetRandomOfArray(ListFoundation)));
            return burgerBuilder.ToString();
        }

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>()
            {
                new Command("Бутерброд","Выдает бутерброд тебе или объекту",
                delegate (OnChatCommandReceivedArgs e) {
                        if(string.IsNullOrEmpty( e.Command.ArgumentsAsString))
                            twitchBot.SendMessage(string.Format("Несу {0} для {1}! HahaCat ", BurgerManager.BurgerCombiner(),e.Command.ChatMessage.DisplayName));
                        else
                            twitchBot.SendMessage(string.Format("Несу {0} для {1}! HahaCat ", BurgerManager.BurgerCombiner(),e.Command.ArgumentsAsString));
                }, new string[] {"Объект"}, CommandType.Interactive)
            };
    }
}