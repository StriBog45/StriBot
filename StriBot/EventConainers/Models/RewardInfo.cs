using StriBot.Bots.Enums;

namespace StriBot.EventConainers.Models
{
    public class RewardInfo
    {
        public Platform Platform { get; set; }

        public string UserName { get; set; }

        public string RewardName { get; set; }

        public string RewardMessage { get; set; }
    }
}