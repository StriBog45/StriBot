using StriBot.Application.FileManager.Models;
using StriBot.Application.Twitch.Interfaces;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace StriBot.Application.Twitch;

public class TwitchInfo : ITwitchInfo
{
    public string Channel { get; private set; }

    public string ChannelId { get; private set; }

    public string ChannelAccessToken { get; private set; }

    public string ChannelRefreshToken { get; private set; }

    public int ChannelExpiresIn { get; private set; }

    public string BotName { get; private set; }

    public string BotAccessToken { get; private set; }

    public string BotRefreshToken { get; private set; }

    public int BotExpiresIn { get; private set; }

    public bool ChannelAuthorized
        => !string.IsNullOrEmpty(ChannelAccessToken);

    public bool BotAuthorized
        => !string.IsNullOrEmpty(BotRefreshToken);

    public void SetChannelToken(AuthCodeResponse authCodeResponse)
    {
        ChannelAccessToken = authCodeResponse.AccessToken;
        ChannelRefreshToken = authCodeResponse.RefreshToken;
        ChannelExpiresIn = authCodeResponse.ExpiresIn;
        BotAccessToken = BotAccessToken ?? authCodeResponse.AccessToken;
    }

    public void SetChannelToken(RefreshResponse refreshResponse)
    {
        ChannelAccessToken = refreshResponse.AccessToken;
        ChannelRefreshToken = refreshResponse.RefreshToken;
        ChannelExpiresIn = refreshResponse.ExpiresIn;
        BotAccessToken = BotAccessToken ?? refreshResponse.AccessToken;
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
        BotRefreshToken = authCodeResponse.RefreshToken;
        BotExpiresIn = authCodeResponse.ExpiresIn;
    }

    public void SetBotToken(RefreshResponse refreshResponse)
    {
        BotAccessToken = refreshResponse.AccessToken;
        BotRefreshToken = refreshResponse.RefreshToken;
        BotExpiresIn = refreshResponse.ExpiresIn;
    }

    public void Set(UserCredentials userCredentials)
    {
        if (userCredentials != null)
        {
            Channel = userCredentials.Channel;
            ChannelId = userCredentials.ChannelId;
            ChannelAccessToken = userCredentials.ChannelAccessToken;
            ChannelRefreshToken = userCredentials.ChannelRefreshToken;
            ChannelExpiresIn = userCredentials.ChannelExpiresIn;
            BotName = userCredentials.BotName;
            BotAccessToken = userCredentials.BotAccessToken;
            BotRefreshToken = userCredentials.BotRefreshToken;
            BotExpiresIn = userCredentials.BotExpiresIn;
        }
    }
}