using System.Threading.Tasks;
using StriBot.Application.Bot;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace StriBot.Application.Authorization
{
    public class TwitchAuthorization
    {
        private const string TwitchRedirectUri = "http://localhost:5278/oauth/";

        private readonly TwitchApiClient _twitchApi;

        public TwitchAuthorization(TwitchApiClient twitchApi)
        {
            _twitchApi = twitchApi;
        }

        public async Task<(AuthCodeResponse, User)> StartAuth()
        {
            var server = new WebServer(TwitchRedirectUri);
            var authorizationModel = await server.Listen();

            server.Stop();

            // exchange auth code for oauth access/refresh
            var authCodeResponse = await _twitchApi.GetAccessToken(authorizationModel.Code, TwitchRedirectUri);
            var user = await _twitchApi.GetAuthorizedUser();

            return (authCodeResponse, user);
        }

        public string GetAuthorizationCodeUrl()
            => _twitchApi.GetAuthorizationCodeUrl(TwitchRedirectUri);
    }
}