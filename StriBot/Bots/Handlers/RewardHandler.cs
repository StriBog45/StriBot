using System.Threading.Tasks;
using StriBot.DateBase;
using StriBot.EventConainers.Models;

namespace StriBot.Bots.Handlers
{
    public class RewardHandler
    {
        private readonly TwitchApiClient _twitchApiClient;

        public RewardHandler(TwitchApiClient twitchApiClient)
        {
            _twitchApiClient = twitchApiClient;
        }

        public async Task Handle(RewardInfo rewardInfo)
        {
            switch (rewardInfo.RewardName)
            {
                case "Перевод баллов":
                    DataBase.AddMoney(rewardInfo.UserName, 5);
                    await _twitchApiClient.CompleteReward(rewardInfo.RewardId, rewardInfo.RedemptionId);
                    break;
            }
        }
    }
}