using StriBot.Application.FileManager.Models;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace StriBot.Application.Twitch.Interfaces;

public interface ITwitchInfo
{
    string Channel { get; }

    string ChannelId { get; }

    string ChannelAccessToken { get; }

    string ChannelRefreshToken { get; }

    int ChannelExpiresIn { get; }

    bool ChannelAuthorized { get; }

    string BotName { get; }

    string BotAccessToken { get; }

    string BotRefreshToken { get; }

    int BotExpiresIn { get; }

    void SetChannelToken(AuthCodeResponse authCodeResponse);

    void SetChannelToken(RefreshResponse refreshResponse);

    void SetStreamerInfo(User streamer);

    void SetBot(AuthCodeResponse authCodeResponse, User userBot);

    void SetBotToken(RefreshResponse refreshResponse);

    void Set(UserCredentials userCredentials);
}