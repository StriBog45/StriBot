using StriBot.Application.Platforms.Enums;

namespace StriBot.Application.Events.Models;

public class RewardInfo
{
    public Platform Platform { get; set; }

    public string UserName { get; set; }

    public string RewardName { get; set; }

    public string RewardMessage { get; set; }

    public string RewardId { get; set; }

    public string RedemptionId { get; set; }
}