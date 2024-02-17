﻿using System.Collections.Generic;
using System.Threading.Tasks;
using StriBot.Application.Bot.Interfaces;
using TwitchLib.Api;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.ChannelPoints.CreateCustomReward;
using TwitchLib.Api.Helix.Models.ChannelPoints.UpdateCustomRewardRedemptionStatus;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace StriBot.Application.Bot
{
    public class TwitchApiClient
    {
        private const string TwitchApplicationClientId = "knbxrofioasvmo625pfga4ccbs3nee";
        private const string TwitchClientSecret = "694l6zbapm00ewmbz97fkf4fyqqmiz";

        private readonly TwitchAPI _twitchApi;
        private readonly ITwitchInfo _twitchInfo;

        public TwitchApiClient(ITwitchInfo twitchInfo)
        {
            _twitchInfo = twitchInfo;
            _twitchApi = new TwitchAPI
            {
                Settings =
                {
                    ClientId = TwitchApplicationClientId,
                    AccessToken = _twitchInfo.ChannelAccessToken
                }
            };
        }

        public Task CompleteReward(string rewardId, string redemptionId)
            => _twitchApi.Helix.ChannelPoints.UpdateRedemptionStatusAsync(_twitchInfo.ChannelId, rewardId,
                new List<string> { redemptionId },
                new UpdateCustomRewardRedemptionStatusRequest { Status = CustomRewardRedemptionStatus.FULFILLED });

        public string GetAuthorizationCodeUrl(string redirectUri)
        {
            var authScopes = new List<AuthScopes>
            {
                AuthScopes.Channel_Subscriptions,
                AuthScopes.Chat_Read,
                AuthScopes.Chat_Edit,
                AuthScopes.User_Blocks_Edit,
                AuthScopes.User_Blocks_Read,
                AuthScopes.User_Follows_Edit,
                AuthScopes.User_Read,
                AuthScopes.User_Subscriptions,
                AuthScopes.Viewing_Activity_Read,
                AuthScopes.Helix_Channel_Manage_Polls,
                AuthScopes.Helix_Channel_Manage_Predictions,
                AuthScopes.Helix_Channel_Manage_Redemptions,
                AuthScopes.Helix_Channel_Read_Polls,
                AuthScopes.Helix_Channel_Read_Predictions,
                AuthScopes.Helix_Channel_Read_Redemptions,
                AuthScopes.Helix_Channel_Read_Subscriptions,
                AuthScopes.Helix_Channel_Read_VIPs,
                AuthScopes.Helix_Moderation_Read,
                AuthScopes.Helix_Moderator_Manage_Banned_Users,
                AuthScopes.Helix_Moderator_Manage_Announcements,
                AuthScopes.Helix_moderator_Manage_Chat_Messages,
                AuthScopes.Helix_Moderator_Manage_Chat_Settings,
                AuthScopes.Helix_Moderator_Read_Chatters,
                AuthScopes.Helix_User_Read_Broadcast,
                AuthScopes.Helix_User_Read_Follows,
                AuthScopes.Helix_User_Read_Subscriptions
            };

            return _twitchApi.Auth.GetAuthorizationCodeUrl(redirectUri, authScopes, forceVerify: true,
                clientId: TwitchApplicationClientId);
        }

        public Task<AuthCodeResponse> GetAccessToken(string code, string redirectUri)
            => _twitchApi.Auth.GetAccessTokenFromCodeAsync(code, TwitchClientSecret, redirectUri);

        public async Task<User> GetAuthorizedUser()
            => (await _twitchApi.Helix.Users.GetUsersAsync()).Users[0];

        public void UpdateAccessToken()
            => _twitchApi.Settings.AccessToken = _twitchInfo.ChannelAccessToken;

        public async Task CreateReward(string name, int price)
            => await _twitchApi.Helix.ChannelPoints.CreateCustomRewardsAsync(_twitchInfo.ChannelId,
                new CreateCustomRewardsRequest
                {
                    Title = name,
                    Cost = price,
                    IsEnabled = false
                });
    }
}