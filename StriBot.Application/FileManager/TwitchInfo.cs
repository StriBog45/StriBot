using StriBot.Application.Bot.Interfaces;
using StriBot.Application.FileManager.Models;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace StriBot.Application.FileManager
{
    public class TwitchInfo : ITwitchInfo
    {
        public string Channel { get; private set; }
        public string ChannelId { get; private set; }
        public string ChannelAccessToken { get; private set; }
        public string BotName { get; private set; }
        public string BotAccessToken { get; private set; }

        public void SetChannelToken(AuthCodeResponse authCodeResponse)
        {
            ChannelAccessToken = authCodeResponse.AccessToken;
            BotAccessToken = BotAccessToken ?? authCodeResponse.AccessToken;
        }

        public void SetStreamerInfo(User streamer)
        {
            Channel = streamer.DisplayName;
            ChannelId = streamer.Id;
            BotName = BotName ?? streamer.DisplayName;
        }

        public void SetBot(AuthCodeResponse authCodeResponse, User userBot)
        {
            BotAccessToken = authCodeResponse.AccessToken;
            BotName = userBot.DisplayName;
        }

        public void Set(UserCredentials userCredentials)
        {
            Channel = userCredentials?.Channel;
            ChannelId = userCredentials?.ChannelId;
            ChannelAccessToken = userCredentials?.ChannelAccessToken;
            BotName = userCredentials?.BotName;
            BotAccessToken = userCredentials?.BotAccessToken;
        }
    }
}