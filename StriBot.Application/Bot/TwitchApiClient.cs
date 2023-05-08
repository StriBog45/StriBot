using System.Collections.Generic;
using System.Threading.Tasks;
using StriBot.Application.Bot.Interfaces;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.ChannelPoints.UpdateCustomRewardRedemptionStatus;

namespace StriBot.Application.Bot
{
    public class TwitchApiClient
    {
        private readonly TwitchAPI _twitchApi;
        private readonly ITwitchInfo _twitchInfo;

        public TwitchApiClient(ITwitchInfo twitchInfo)
        {
            _twitchInfo = twitchInfo;
            _twitchApi = new TwitchAPI
            {
                Settings =
                {
                    ClientId = _twitchInfo.ChannelClientId,
                    AccessToken = _twitchInfo.ChannelAccessToken
                },
            };
        }

        public async Task CompleteReward(string rewardId, string redemptionId)
            => await _twitchApi.Helix.ChannelPoints.UpdateRedemptionStatusAsync(_twitchInfo.ChannelId, rewardId,
                new List<string> {redemptionId},
                new UpdateCustomRewardRedemptionStatusRequest {Status = CustomRewardRedemptionStatus.FULFILLED});

        //string GetUptime()
        //{
        //    string userId = GetUserId(_twitchInfo.Channel);

        //    return string.IsNullOrEmpty(userId) ? "Offline" : _api.V5.Streams.GetUptimeAsync(userId).Result.Value.ToString(@"hh\:mm\:ss");
        //}

        //string GetUserId(string username)
        //{
        //    var userList = _api.V5.Users.GetUserByNameAsync(username).Result.Matches;
        //    return userList[0]?.Id;
        //}
    }
}