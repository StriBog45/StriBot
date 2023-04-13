using System.Threading.Tasks;
using StriBot.Application.Commands.Handlers;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;
using StriBot.Application.Localization.Extensions;
using StriBot.Application.Localization.Implementations;

namespace StriBot.Application.Bot.Handlers
{
    public class RewardHandler
    {
        private readonly TwitchApiClient _twitchApiClient;
        private readonly IDataBase _dataBase;
        private readonly Currency _currency;
        private readonly RememberHandler _rememberHandler;
        private readonly BananaHandler _bananaHandler;

        public RewardHandler(TwitchApiClient twitchApiClient,
            IDataBase dataBase,
            Currency currency,
            RememberHandler rememberHandler,
            BananaHandler bananaHandler)
        {
            _twitchApiClient = twitchApiClient;
            _dataBase = dataBase;
            _currency = currency;
            _rememberHandler = rememberHandler;
            _bananaHandler = bananaHandler;
        }

        public async Task Handle(RewardInfo rewardInfo)
        {
            switch (rewardInfo.RewardName)
            {
                case "Перевод баллов":
                    _dataBase.AddMoney(rewardInfo.UserName, 5);
                    await _twitchApiClient.CompleteReward(rewardInfo.RewardId, rewardInfo.RedemptionId);
                    break;
                case "#1":
                    _dataBase.AddMoney(rewardInfo.UserName, 5);
                    EventContainer.Message($"{rewardInfo.UserName} сегодня первый зритель нашей трансляции! Держи за это {_currency.Incline(5)}", rewardInfo.Platform);
                    _rememberHandler.SetFirstViewer(rewardInfo.UserName);
                    await _twitchApiClient.CompleteReward(rewardInfo.RewardId, rewardInfo.RedemptionId);
                    break;
                case "УВЕЛМЧЕНИЕ БАНАНА":
                    _bananaHandler.IncreaseBananaSize(rewardInfo.UserName);
                    break;
            }
        }
    }
}