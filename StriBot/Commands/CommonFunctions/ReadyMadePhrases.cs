using StriBot.Bots.Enums;
using StriBot.EventConainers;
using StriBot.Language;

namespace StriBot.Commands.CommonFunctions
{
    public class ReadyMadePhrases
    {
        private readonly Currency currency;

        public ReadyMadePhrases(Currency currency)
        {
            this.currency = currency;
        }

        public void NoMoney(string displayName, Platform platform)
            => GlobalEventContainer.Message($"{displayName} у вас недостаточно {currency.GenitiveMultiple}! striboCry ", platform);

        public void IncorrectCommand(Platform platform)
            => GlobalEventContainer.Message("Некорректное использование команды!", platform);
    }
}