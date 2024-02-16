using StriBot.Application.FileManager.Models;
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

        string ChannelId { get; }

        void SetChannelToken(AuthCodeResponse authCodeResponse);

        void SetStreamerInfo(User streamer);

        void SetBot(AuthCodeResponse authCodeResponse, User userBot);

        void Set(UserCredentials userCredentials);
    }
}