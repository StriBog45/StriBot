using System.Globalization;

namespace StriBot.Language.Extensions
{
    public static class StringExtension
    {
        public static string Title(this string text)
        {
            var result = string.Empty;

            if (!string.IsNullOrEmpty(text))
            {
                var textInfo = new CultureInfo("ru-RU").TextInfo;
                result = textInfo.ToTitleCase(text);
            }

            return result;
        }
    }
}