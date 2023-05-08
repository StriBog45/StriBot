namespace StriBot.Application.Bot.Interfaces
{
    public interface ITwitchInfo
    {
        string Channel { get; }

        string BotAccessToken { get; }

        string ChannelAccessToken { get; }

        string BotName { get; }

        string ChannelClientId { get; }

        string ChannelId { get; }
    }
}