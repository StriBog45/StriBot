using StriBot.Language.Interfaces;

namespace StriBot.Language.Extensions
{
    public static class CasesExtension
    {
        /// <summary>
        /// Склоняет текс. Пример: 5 минут
        /// </summary>
        /// <param name="cases"></param>
        /// <param name="number"></param>
        /// <param name="secondDeclension"></param>
        /// <returns></returns>
        public static string Incline(this ICases cases, int number, bool secondDeclension = false)
        {
            string result = string.Empty;

            if (number == 1 && !secondDeclension)
                result = cases.Nominative;
            else if (number == 1 && secondDeclension)
                result = cases.Accusative;
            else if (number >= 2 && number <= 4)
                result = cases.Genitive;
            else if (number >= 5 && number <= 20)
                result = cases.GenitiveMultiple;
            else if (number % 10 == 1 && !secondDeclension)
                result = cases.Nominative;
            else if (number % 10 == 1 && secondDeclension)
                result = cases.Accusative;
            else if (number % 10 >= 2 && number % 10 <= 4)
                result = cases.Genitive;
            else if (number % 10 >= 5 && number % 10 <= 9 || number % 10 == 0)
                result = cases.GenitiveMultiple;

            return $"{number} {result}";
        }
    }
}