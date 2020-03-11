namespace StriBot.Language
{
    public static class CasesExtension
    {
        public static string Incline(this Cases cases, int number, bool secondDeclension = false)
        {
            string result = string.Empty;

            if(number == 1 && !secondDeclension)
                result = cases.Nominative;
            else if (number == 1 && secondDeclension)
                result = cases.Dative;
            else if(number >= 2 && number <= 4)
                result = cases.NominativeMultiple;
            else if (number >= 5 && number <= 20)
                result = cases.GenitiveMultiple;
            else if (number % 10 == 1 && !secondDeclension)
                result = cases.Nominative;
            else if (number % 10 == 1 && secondDeclension)
                result = cases.Dative;
            else if (number % 10 >= 2 && number % 10 <= 4)
                result = cases.NominativeMultiple;
            else if (number % 10 >= 5 && number % 10 <= 9 || number % 10 == 0)
                result = cases.GenitiveMultiple;

            return result;
        }
    }
}