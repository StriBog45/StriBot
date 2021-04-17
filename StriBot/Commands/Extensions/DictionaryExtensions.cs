using StriBot.Commands.Models;
using System.Collections.Generic;

namespace StriBot.Commands.Extensions
{
    public static class DictionaryExtensions
    {
        public static void Add(this Dictionary<string, Command> current, Dictionary<string, Command> dictionary)
        {
            foreach(var item in dictionary)
            {
                current.Add(item.Key, item.Value);
            }
        }

        public static void Add(this Dictionary<string, Command> current, Command command)
            => current.Add(command.Name.ToLower(), command);
    }
}