using Microsoft.Extensions.Configuration;
using StriBot.Application.Bot.Interfaces;
using StriBot.Application.Configurations.Models;

namespace StriBot.Application.FileManager
{
    public class TwitchInfo : ITwitchInfo
    {
        public string Channel { get; }
        public string ChannelId { get; }
        public string ChannelClientId { get; }
        public string ChannelAccessToken { get; }
        public string BotName { get; }
        public string BotClientId { get; }
        public string BotAccessToken { get; }

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
    }
}