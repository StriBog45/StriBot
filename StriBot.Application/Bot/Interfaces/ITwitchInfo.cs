using TwitchLib.Api.Auth;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

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

        void SetChannel(AuthCodeResponse authCodeResponse, User streamer);

        void SetBot(AuthCodeResponse authCodeResponse, User userBot);
    }
}