using System.Collections.Generic;
using StriBot.Application.Bot.Enums;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Commands.Extensions;
using StriBot.Application.Commands.Models;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;
using StriBot.Application.Platforms.Enums;

namespace StriBot.Application.Commands.Handlers
{
    public class RememberHandler
    {
        public string TextReminder { get; set; } = string.Empty;
        private Platform _platform;

        public Dictionary<string, Command> CreateCommands()
            => new()
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
                        EventContainer.Message($"Напоминание: \"{commandInfo.ArgumentsAsString}\" создано", commandInfo.Platform);
                        _platform = commandInfo.Platform;
                    }
                    else
                        EventContainer.Message("Напоминание удалено", commandInfo.Platform);
                }, new [] { "текст" }, CommandType.Interactive);

            return result;
        }

        public void Tick(int timer)
        {
            if (timer % 10 == 0 && !string.IsNullOrEmpty(TextReminder))
                EventContainer.Message("Напоминание: " + TextReminder, _platform);
        }
    }
}