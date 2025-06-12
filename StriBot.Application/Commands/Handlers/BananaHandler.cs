using System.Collections.Generic;
using StriBot.Application.Bot.Enums;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Commands.Extensions;
using StriBot.Application.Commands.Models;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;
using StriBot.Application.Localization;

namespace StriBot.Application.Commands.Handlers;

public class BananaHandler
{
    private readonly IDataBase _dataBase;

    public BananaHandler(IDataBase dataBase)
    {
        _dataBase = dataBase;
    }

    public Dictionary<string, Command> CreateCommands()
        => new Dictionary<string, Command>
        {
            GetMySize(),
            GetTopBananas(),
            SetBananaSize()
        };

    private Command GetMySize()
        => new Command("mysize", "Узнать размер банана",
            delegate (CommandInfo commandInfo)
            {
                var bananaSize = _dataBase.GetBananaSize(commandInfo.DisplayName);
                EventContainer.Message($"Твой банан {bananaSize}", commandInfo.Platform);
            }, CommandType.Interactive);

    private Command GetTopBananas()
        => new Command("topbanana", "Топ бананов",
            delegate (CommandInfo commandInfo)
            {
                var bananaInfos = _dataBase.GetTopBananas();
                var places = new List<string>();

                var place = 1;
                foreach (var bananaInfo in bananaInfos)
                {
                    places.Add($"{place}. {bananaInfo.Nick} - {bananaInfo.BananaSize}");
                    place++;
                }

                EventContainer.Message(string.Join(", ", places), commandInfo.Platform);
            }, new []{"Ник", "Размер"}, CommandType.Interactive);

    public void IncreaseBananaSize(string nick)
        => _dataBase.IncreaseBananaSize(nick);

    private Command SetBananaSize()
    {
        return new Command("setbanana", "Установить размер банана. Доступно только для владельца канала", Role.Broadcaster, Action, CommandType.Interactive);

        void Action(CommandInfo commandInfo)
        {
            if (commandInfo.ArgumentsAsList.Count == 2 && int.TryParse(commandInfo.ArgumentsAsList[1], out var bananaSize))
            {
                _dataBase.SetBananaSize(commandInfo.ArgumentsAsList[0], bananaSize);
                EventContainer.Message($"Установлен размер банана {bananaSize} для {commandInfo.ArgumentsAsList[0]}!", commandInfo.Platform);
            }
            else
                ReadyMadePhrases.IncorrectCommand(commandInfo.Platform);
        }
    }
}