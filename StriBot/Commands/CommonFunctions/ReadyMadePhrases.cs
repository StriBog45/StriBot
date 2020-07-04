using StriBot.Language;
using StriBot.TwitchBot.Interfaces;

namespace StriBot.Commands.CommonFunctions
{
    public class ReadyMadePhrases
    {
        private readonly ITwitchBot twitchBot;
        private readonly Currency currency;

        public ReadyMadePhrases(ITwitchBot twitchBot, Currency currency)
        {
            this.twitchBot = twitchBot;
            this.currency = currency;
        }

        public void NoMoney(string displayName)
            => twitchBot.SendMessage($"{displayName} у вас недостаточно {currency.GenitiveMultiple}! striboCry ");

        public void IncorrectCommand()
            => twitchBot.SendMessage("Некорректное использование команды!");
    }
}