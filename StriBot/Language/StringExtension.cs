using System.Globalization;

namespace StriBot.Language
{
    public static class StringExtension
    {
        public static string Title(this string text)
        {
            var textInfo = new CultureInfo("ru-RU").TextInfo;
            return textInfo.ToTitleCase(text);
        }
    }
}