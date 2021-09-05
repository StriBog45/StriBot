using StriBot.Bots.Enums;
using StriBot.Commands.Enums;
using StriBot.Commands.Extensions;
using StriBot.Commands.Models;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using System.Collections.Generic;

namespace StriBot.Commands
{
    public class RememberManager
    {
        public string TextReminder { get; set; } = string.Empty;
        private Platform _platform;

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>
            {
                CreateRemindCommand()
            };

        private Command CreateRemindCommand()
        {
            var result = new Command("Напомнить", "Создает напоминалку. При использовании без указания текста, напоминание будет удалено", Role.Moderator,
                delegate (CommandInfo commandInfo)
                {
                    TextReminder = commandInfo.ArgumentsAsString;
                    if (TextReminder.Length > 0)
                    {
                        GlobalEventContainer.Message($"Напоминание: \"{commandInfo.ArgumentsAsString}\" создано", commandInfo.Platform);
                        _platform = commandInfo.Platform;
                    }
                    else
                        GlobalEventContainer.Message("Напоминание удалено", commandInfo.Platform);
                }, new [] { "текст" }, CommandType.Interactive);

            return result;
        }

        public void Tick(int timer)
        {
            if (timer % 10 == 0 && !string.IsNullOrEmpty(TextReminder))
                GlobalEventContainer.Message("Напоминание: " + TextReminder, _platform);
        }
    }
}