using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace StriBot.Application.Authorization
{
    public class TwitchAuthorization
    {
        private const string TwitchClientId = "knbxrofioasvmo625pfga4ccbs3nee";
        private const string TwitchRedirectUri = "http://localhost:5278/oauth/";
        private const string TwitchClientSecret = "694l6zbapm00ewmbz97fkf4fyqqmiz";

        private readonly TwitchAPI _twitchApi;

        public TwitchAuthorization()
        {
            ValidateApplicationCredentials();

            _twitchApi = new TwitchAPI
            {
                Settings =
                {
                    ClientId = TwitchClientId
                }
            };
        }

        public async Task<(AuthCodeResponse, User)> StartAuth()
        {
            // start local web server
            var server = new WebServer(TwitchRedirectUri);

            // listen for incoming requests
            var authorizationModel = await server.Listen();

            server.Stop();

            // exchange auth code for oauth access/refresh
            var authCodeResponse = await _twitchApi.Auth.GetAccessTokenFromCodeAsync(authorizationModel.Code, TwitchClientSecret, TwitchRedirectUri);

            // update TwitchLib's api with the recently acquired access token
            _twitchApi.Settings.AccessToken = authCodeResponse.AccessToken;

            // get the authorized user
            var user = (await _twitchApi.Helix.Users.GetUsersAsync()).Users[0];

            return (authCodeResponse, user);
        }

        private string GetAuthorizationCodeUrl(string clientId, string redirectUri)
        {
            var authScopes = new List<AuthScopes>
            {
                AuthScopes.Channel_Check_Subscription,
                AuthScopes.Channel_Commercial,
                AuthScopes.Channel_Editor,
                AuthScopes.Channel_Feed_Edit,
                AuthScopes.Channel_Feed_Read,
                AuthScopes.Channel_Read,
                AuthScopes.Channel_Stream,
                AuthScopes.Channel_Subscriptions,
                AuthScopes.Chat_Read,
                AuthScopes.Chat_Edit,
                AuthScopes.Collections_Edit,
                AuthScopes.Communities_Edit,
                AuthScopes.Communities_Moderate,
                AuthScopes.User_Blocks_Edit,
                AuthScopes.User_Blocks_Read,
                AuthScopes.User_Follows_Edit,
                AuthScopes.User_Read,
                AuthScopes.User_Subscriptions,
                AuthScopes.Viewing_Activity_Read,
                AuthScopes.Helix_Analytics_Read_Extensions,
                AuthScopes.Helix_Analytics_Read_Games,
                AuthScopes.Helix_Bits_Read,
                AuthScopes.Helix_Channel_Edit_Commercial,
                AuthScopes.Helix_Channel_Manage_Broadcast,
                AuthScopes.Helix_Channel_Manage_Extensions,
                AuthScopes.Helix_Channel_Manage_Moderators,
                AuthScopes.Helix_Channel_Manage_Polls,
                AuthScopes.Helix_Channel_Manage_Predictions,
                AuthScopes.Helix_Channel_Manage_Redemptions,
                AuthScopes.Helix_Channel_Manage_Schedule,
                AuthScopes.Helix_Channel_Manage_Videos,
                AuthScopes.Helix_Channel_Manage_VIPs,
                AuthScopes.Helix_Channel_Read_Charity,
                AuthScopes.Helix_Channel_Read_Editors,
                AuthScopes.Helix_Channel_Read_Goals,
                AuthScopes.Helix_Channel_Read_Hype_Train,
                AuthScopes.Helix_Channel_Read_Polls,
                AuthScopes.Helix_Channel_Read_Predictions,
                AuthScopes.Helix_Channel_Read_Redemptions,
                AuthScopes.Helix_Channel_Read_Stream_Key,
                AuthScopes.Helix_Channel_Read_Subscriptions,
                AuthScopes.Helix_Channel_Read_VIPs,
                AuthScopes.Helix_Clips_Edit,
                AuthScopes.Helix_Moderation_Read,
                AuthScopes.Helix_Moderator_Manage_Banned_Users,
                AuthScopes.Helix_Moderator_Manage_Blocked_Terms,
                AuthScopes.Helix_Moderator_Manage_Announcements,
                AuthScopes.Helix_Moderator_Manage_Automod,
                AuthScopes.Helix_Moderator_Manage_Automod_Settings,
                AuthScopes.Helix_moderator_Manage_Chat_Messages,
                AuthScopes.Helix_Moderator_Manage_Chat_Settings,
                AuthScopes.Helix_Moderator_Read_Blocked_Terms,
                AuthScopes.Helix_Moderator_Read_Automod_Settings,
                AuthScopes.Helix_Moderator_Read_Chat_Settings,
                AuthScopes.Helix_Moderator_Read_Chatters,
                AuthScopes.Helix_User_Edit,
                AuthScopes.Helix_User_Edit_Broadcast,
                AuthScopes.Helix_User_Edit_Follows,
                AuthScopes.Helix_User_Manage_BlockedUsers,
                AuthScopes.Helix_User_Manage_Chat_Color,
                AuthScopes.Helix_User_Manage_Whispers,
                AuthScopes.Helix_User_Read_BlockedUsers,
                AuthScopes.Helix_User_Read_Broadcast,
                AuthScopes.Helix_User_Read_Email,
                AuthScopes.Helix_User_Read_Follows,
                AuthScopes.Helix_User_Read_Subscriptions,
            };

            return _twitchApi.Auth.GetAuthorizationCodeUrl(redirectUri, authScopes, forceVerify: true,
                clientId: clientId);
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

        public string GetAuthorizationCodeUrl()
            => GetAuthorizationCodeUrl(TwitchClientId, TwitchRedirectUri);

        public async Task<RefreshResponse> RefreshUser(string refreshToken)
        {
            // ensure client id, secret, and redirect url are set
            ValidateApplicationCredentials();

            // refresh token
            var refresh = await _twitchApi.Auth.RefreshAuthTokenAsync(refreshToken, TwitchClientSecret);
            _twitchApi.Settings.AccessToken = refresh.AccessToken;

            // confirm new token works
            var user = (await _twitchApi.Helix.Users.GetUsersAsync()).Users[0];

            // print out all the data we've got
            Console.WriteLine($"Authorization success!\n\nUser: {user.DisplayName} (id: {user.Id})\nAccess token: {refresh.AccessToken}\nRefresh token: {refresh.RefreshToken}\nExpires in: {refresh.ExpiresIn}\nScopes: {string.Join(", ", refresh.Scopes)}");

            return refresh;
        }
    }
}