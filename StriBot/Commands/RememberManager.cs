using StriBot.Bots.Enums;
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
        {
            var result = new Dictionary<string, Command>();
            result.Add(CreateRemindCommand());

            return result;
        }

        public Command CreateRemindCommand()
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
                }, new string[] { "текст" }, CommandType.Interactive);

            return result;
        }

        public void Tick(int timer)
        {
            if (timer % 10 == 0 && !string.IsNullOrEmpty(TextReminder))
                GlobalEventContainer.Message("Напоминание: " + TextReminder, _platform);
        }
    }
}