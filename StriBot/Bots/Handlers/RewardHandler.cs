using StriBot.DateBase;
using StriBot.EventConainers.Models;

namespace StriBot.Bots.Handlers
{
    public class RewardHandler
    {
        public void Handle(RewardInfo rewardInfo)
        {
            switch (rewardInfo.RewardName)
            {
                case "Перевод баллов":
                    DataBase.AddMoney(rewardInfo.UserName, 5);
                    break;
            }
        }
    }
}