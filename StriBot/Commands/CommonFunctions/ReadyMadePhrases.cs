using StriBot.Application.Events;
using StriBot.Application.Localization.Implementations;
using StriBot.Application.Platforms.Enums;

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
            => EventContainer.Message($"{displayName} у вас недостаточно {_currency.GenitiveMultiple}! striboCry ", platform);

        public static void IncorrectCommand(Platform platform)
            => EventContainer.Message("Некорректное использование команды!", platform);
    }
}