using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.ChannelPoints.UpdateCustomRewardRedemptionStatus;

namespace StriBot.Bots
{
    public class TwitchApiClient
    {
        private readonly TwitchAPI _twitchApi;
        private readonly TwitchInfo _twitchInfo;

        public TwitchApiClient()
        {
            _twitchInfo = new TwitchInfo();
            _twitchApi = new TwitchAPI
            {
                Settings =
                {
                    ClientId = _twitchInfo.BotClientId,
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