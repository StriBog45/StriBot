using System.Collections.Generic;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Commands.Extensions;
using StriBot.Application.Commands.Models;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;

namespace StriBot.Application.Commands.Handlers
{
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
                GetTopBananas()
            };

        private Command GetMySize()
            => new Command("mysize", "Узнать размер банана",
                delegate (CommandInfo commandInfo)
                {
                    var bananaSize = _dataBase.GetBananaSize(commandInfo.DisplayName);
                    EventContainer.Message($"Твой банан {bananaSize}", commandInfo.Platform);
                }, CommandType.Info);

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
                }, CommandType.Info);

        public void IncreaseBananaSize(string nick)
            => _dataBase.IncreaseBananaSize(nick);
    }
}