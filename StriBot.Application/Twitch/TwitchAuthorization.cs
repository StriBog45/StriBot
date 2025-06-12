using System.Threading.Tasks;
using StriBot.Application.Server;
using StriBot.Application.Twitch.Interfaces;
using TwitchLib.Api.Auth;

namespace StriBot.Application.Twitch;

public class TwitchAuthorization
{
    private const string TwitchRedirectUri = "http://localhost:5278/oauth/";

    private readonly TwitchApiClient _twitchApiClient;
    private readonly ITwitchInfo _twitchInfo;

    public TwitchAuthorization(TwitchApiClient twitchApiClient, ITwitchInfo twitchInfo)
    {
        _twitchApiClient = twitchApiClient;
        _twitchInfo = twitchInfo;
    }

    public async Task<AuthCodeResponse> StartAuth()
    {
        var server = new WebServer(TwitchRedirectUri);
        var authorizationModel = await server.Listen();

        server.Stop();

        // exchange auth code for oauth access/refresh
        var authCodeResponse = await _twitchApiClient.GetAccessToken(authorizationModel.Code, TwitchRedirectUri);

        return authCodeResponse;
    }

    public string GetAuthorizationCodeUrl()
        => _twitchApiClient.GetAuthorizationCodeUrl(TwitchRedirectUri);

    public async Task RefreshTokens()
    {
        if (!string.IsNullOrEmpty(_twitchInfo.ChannelRefreshToken))
        {
            var refreshResponse = await _twitchApiClient.RefreshToken(_twitchInfo.ChannelRefreshToken);
            _twitchInfo.SetChannelToken(refreshResponse);
        }

        if (!string.IsNullOrEmpty(_twitchInfo.BotRefreshToken))
        {
            var refreshResponse = await _twitchApiClient.RefreshToken(_twitchInfo.BotRefreshToken);
            _twitchInfo.SetBotToken(refreshResponse);
        }
    }
}