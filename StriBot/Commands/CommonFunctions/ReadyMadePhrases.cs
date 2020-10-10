using StriBot.Bots.Enums;
using StriBot.EventConainers;
using StriBot.Language.Implementations;

namespace StriBot.Commands.CommonFunctions
{
    public class ReadyMadePhrases
    {
        private readonly Currency _currency;

        public ReadyMadePhrases(Currency currency)
        {
            _currency = currency;
        }

        public void NoMoney(string displayName, Platform platform)
            => GlobalEventContainer.Message($"{displayName} у вас недостаточно {_currency.GenitiveMultiple}! striboCry ", platform);

        public void IncorrectCommand(Platform platform)
            => GlobalEventContainer.Message("Некорректное использование команды!", platform);
    }
}