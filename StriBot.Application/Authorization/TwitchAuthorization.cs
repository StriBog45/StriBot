using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StriBot.Application.Authorization
{
    public class TwitchAuthorization
    {
        private static readonly List<string> Scopes = new List<string> { "chat:read", "chat:edit", "channel:moderate" };

        private const string TwitchClientId = "";
        private const string TwitchRedirectUri = "http://localhost:5278/oauth/";
        private const string TwitchClientSecret = "";

        public static async Task StartAuth()
        {
            // ensure client id, secret, and redirect url are set
            ValidateApplicationCredentials();

            // create twitch api instance
            var api = new TwitchLib.Api.TwitchAPI
            {
                Settings =
                {
                    ClientId = TwitchClientId
                }
            };

            // start local web server
            var server = new WebServer(TwitchRedirectUri);

            // listen for incoming requests
            var authorizationModel = await server.Listen();

            // exchange auth code for oauth access/refresh
            var authCodeResponse = await api.Auth.GetAccessTokenFromCodeAsync(authorizationModel.Code, TwitchClientSecret, TwitchRedirectUri);

            // update TwitchLib's api with the recently acquired access token
            api.Settings.AccessToken = authCodeResponse.AccessToken;

            // get the authorized user
            var user = (await api.Helix.Users.GetUsersAsync()).Users[0];

            // print out all the data we've got
            Console.WriteLine($"Authorization success!\n\nUser: {user.DisplayName} (id: {user.Id})\nAccess token: {authCodeResponse.AccessToken}\nRefresh token: {authCodeResponse.RefreshToken}\nExpires in: {authCodeResponse.ExpiresIn}\nScopes: {string.Join(", ", authCodeResponse.Scopes)}");

            // refresh token
            var refresh = await api.Auth.RefreshAuthTokenAsync(authCodeResponse.RefreshToken, TwitchClientSecret);
            api.Settings.AccessToken = refresh.AccessToken;

            // confirm new token works
            user = (await api.Helix.Users.GetUsersAsync()).Users[0];

            // print out all the data we've got
            Console.WriteLine($"Authorization success!\n\nUser: {user.DisplayName} (id: {user.Id})\nAccess token: {refresh.AccessToken}\nRefresh token: {refresh.RefreshToken}\nExpires in: {refresh.ExpiresIn}\nScopes: {string.Join(", ", refresh.Scopes)}");
        }

        private static string GetAuthorizationCodeUrl(string clientId, string redirectUri, IEnumerable<string> scopes)
        {
            var scopesStr = string.Join("+", scopes);

            return "https://id.twitch.tv/oauth2/authorize?" +
                   $"client_id={clientId}&" +
                   $"redirect_uri={System.Web.HttpUtility.UrlEncode(redirectUri)}&" +
                   "response_type=code&" +
                   $"scope={scopesStr}";
        }

        private static void ValidateApplicationCredentials()
        {
            if (string.IsNullOrEmpty(TwitchClientId))
                throw new Exception("client id cannot be null or empty");
            if (string.IsNullOrEmpty(TwitchClientSecret))
                throw new Exception("client secret cannot be null or empty");
            if (string.IsNullOrEmpty(TwitchRedirectUri))
                throw new Exception("redirect uri cannot be null or empty");
        }

        public static string GetAuthorizationCodeUrl()
            => GetAuthorizationCodeUrl(TwitchClientId, TwitchRedirectUri, Scopes);
    }
}