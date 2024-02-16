using System.Threading.Tasks;
using StriBot.Application.Bot;
using TwitchLib.Api.Auth;

namespace StriBot.Application.Authorization
{
    public class TwitchAuthorization
    {
        private const string TwitchRedirectUri = "http://localhost:5278/oauth/";

        private readonly TwitchApiClient _twitchApiClient;

        public TwitchAuthorization(TwitchApiClient twitchApiClient)
        {
            _twitchApiClient = twitchApiClient;
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
    }
}