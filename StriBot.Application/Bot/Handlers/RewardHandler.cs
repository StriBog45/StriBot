using System.Threading.Tasks;
using StriBot.Application.Commands.Handlers;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.Events.Models;
using StriBot.Application.Twitch;

namespace StriBot.Application.Bot.Handlers;

public class RewardHandler
{
    private readonly TwitchApiClient _twitchApiClient;
    private readonly IDataBase _dataBase;
    private readonly RememberHandler _rememberHandler;
    private readonly BananaHandler _bananaHandler;

    public RewardHandler(TwitchApiClient twitchApiClient,
        IDataBase dataBase,
        RememberHandler rememberHandler,
        BananaHandler bananaHandler)
    {
        _twitchApiClient = twitchApiClient;
        _dataBase = dataBase;
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
                _rememberHandler.SetFirstViewer(rewardInfo.UserName);
                await _twitchApiClient.CompleteReward(rewardInfo.RewardId, rewardInfo.RedemptionId);
                break;
            case "УВЕЛМЧЕНИЕ БАНАНА":
                _bananaHandler.IncreaseBananaSize(rewardInfo.UserName);
                await _twitchApiClient.CompleteReward(rewardInfo.RewardId, rewardInfo.RedemptionId);
                break;
        }
    }
}