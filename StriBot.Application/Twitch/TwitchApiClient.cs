using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StriBot.Application.Events;
using StriBot.Application.Twitch.Interfaces;
using TwitchLib.Api;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Core.Exceptions;
using TwitchLib.Api.Helix.Models.ChannelPoints.CreateCustomReward;
using TwitchLib.Api.Helix.Models.ChannelPoints.UpdateCustomRewardRedemptionStatus;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace StriBot.Application.Twitch;

public class TwitchApiClient
{
    // Get ClientId and ClientSecret by register an Application here: https://dev.twitch.tv/console/apps
    // https://dev.twitch.tv/docs/authentication/register-app/
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
                ClientId = TwitchApplicationClientId
            }
        };
    }

    public async Task CompleteReward(string rewardId, string redemptionId)
    {
        try
        {
            await _twitchApi.Helix.ChannelPoints.UpdateRedemptionStatusAsync(_twitchInfo.ChannelId, rewardId,
                [redemptionId],
                new UpdateCustomRewardRedemptionStatusRequest { Status = CustomRewardRedemptionStatus.FULFILLED },
                accessToken:_twitchInfo.ChannelAccessToken);
        }
        catch (BadTokenException)
        {
            EventContainer.MessageToApplication("Бот не может закрыть награду, которую создал не бот. Создайте награду из приложения", "Невозможно закрыть награду");
        }
        catch (Exception e)
        {
            EventContainer.MessageToApplication(e.ToString(), "Ошибка");
        }
    }

    public string GetAuthorizationCodeUrl(string redirectUri)
    {
        var authScopes = new List<AuthScopes>
        {
            AuthScopes.Channel_Feed_Edit,
            AuthScopes.Channel_Feed_Read,
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

    public Task<ValidateAccessTokenResponse> ValidateAccessToken(string code)
        => _twitchApi.Auth.ValidateAccessTokenAsync(code);

    public async Task<User> GetAuthorizedUser()
        => (await _twitchApi.Helix.Users.GetUsersAsync(accessToken: _twitchInfo.ChannelAccessToken)).Users[0];

    public async Task CreateReward(string name, int price)
        => await _twitchApi.Helix.ChannelPoints.CreateCustomRewardsAsync(_twitchInfo.ChannelId,
            new CreateCustomRewardsRequest
            {
                Title = name,
                Cost = price,
                IsEnabled = false
            },
            accessToken: _twitchInfo.ChannelAccessToken);

    public Task<RefreshResponse> RefreshToken(string refreshToken)
        => _twitchApi.Auth.RefreshAuthTokenAsync(refreshToken, TwitchClientSecret);

    public async Task SubscribeChannelPointsCustomRewardRedemptionAddAsync(string sessionId)
    {
        // https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelchannel_points_custom_reward_redemptionadd
        var condition = new Dictionary<string, string> { { "broadcaster_user_id", _twitchInfo.ChannelId } };

        await _twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.channel_points_custom_reward_redemption.add", "1", condition,
            EventSubTransportMethod.Websocket, sessionId, clientId: TwitchApplicationClientId, accessToken: _twitchInfo.ChannelAccessToken);
    }
}