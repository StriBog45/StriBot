using Microsoft.Extensions.Configuration;
using StriBot.Application.Bot.Interfaces;
using StriBot.Application.Configurations.Models;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace StriBot.Application.FileManager
{
    public class TwitchInfo : ITwitchInfo
    {
        public string Channel { get; private set; }
        public string ChannelId { get; private set; }
        public string ChannelClientId { get; private set; }
        public string ChannelAccessToken { get; private set; }
        public string BotName { get; private set; }
        public string BotClientId { get; private set; }
        public string BotAccessToken { get; private set; }

        public TwitchInfo(IConfiguration configuration)
        {
            var channelSettings = configuration.GetSection(nameof(ChannelSettings)).Get<ChannelSettings>();
            var botSettings = configuration.GetSection(nameof(BotSettings)).Get<BotSettings>();

            Channel = channelSettings.ChannelName;
            ChannelId = channelSettings.ChannelId;
            ChannelClientId = channelSettings.ClientId;
            ChannelAccessToken = channelSettings.AccessToken;

            BotName = botSettings.ChannelName;
            BotClientId = botSettings.ClientId;
            BotAccessToken = botSettings.AccessToken;
        }

        public void SetChannel(AuthCodeResponse authCodeResponse, User streamer)
        {
            ChannelAccessToken = authCodeResponse.AccessToken;
            BotAccessToken = authCodeResponse.AccessToken;
            Channel = streamer.DisplayName;
            ChannelId = streamer.Id;
        }

        public void SetBot(AuthCodeResponse authCodeResponse, User userBot)
        {
            BotAccessToken = authCodeResponse.AccessToken;
            BotName = userBot.DisplayName;
            BotClientId = userBot.Id;
        }
    }
}