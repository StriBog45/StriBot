using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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