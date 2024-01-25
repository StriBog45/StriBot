using System.Collections.Generic;
using StriBot.Application.Bot.Enums;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Commands.Extensions;
using StriBot.Application.Commands.Models;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;
using StriBot.Application.Localization.Implementations;
using StriBot.Application.Platforms.Enums;

namespace StriBot.Application.Commands.Handlers
{
    public class RememberHandler
    {
        public string TextReminder { get; set; } = string.Empty;
        private string _firstViewer;
        private Platform _platform;
        private readonly IDataBase _dataBase;
        private readonly Currency _currency;

        public RememberHandler(IDataBase dataBase, Currency currency)
        {
            _dataBase = dataBase;
            _currency = currency;
        }

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command> { CreateRemindCommand() };

        private Command CreateRemindCommand()
        {
            var result = new Command("Напомнить", "Создает напоминание. При использовании без указания текста, напоминание будет удалено", Role.Moderator,
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
                EventContainer.Message($"Напоминание: {TextReminder}", _platform);
            if (timer % 30 == 0 && !string.IsNullOrEmpty(_firstViewer))
                EventContainer.Message($"Первый зритель нашей трансляции: {_firstViewer}! hugBack ", _platform);
        }

        public void SetFirstViewer(string name)
        {
            _dataBase.AddMoney(name, 5);
            _dataBase.IncreaseFirstViewerTimes(name);
            var firstViewerTimes = _dataBase.GetFirstViewerTimes(name);
            EventContainer.Message($"{name} первый зритель трансляции! Получил награду в {firstViewerTimes}-й раз!", Platform.Twitch);
            _firstViewer = name;
        }
    }
}