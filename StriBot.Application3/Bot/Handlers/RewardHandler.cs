﻿using System.Threading.Tasks;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.Events.Models;

namespace StriBot.Application.Bot.Handlers
{
    public class RewardHandler
    {
        private readonly TwitchApiClient _twitchApiClient;
        private readonly IDataBase _dataBase;

        public RewardHandler(TwitchApiClient twitchApiClient, IDataBase dataBase)
        {
            _twitchApiClient = twitchApiClient;
            _dataBase = dataBase;
        }

        public async Task Handle(RewardInfo rewardInfo)
        {
            switch (rewardInfo.RewardName)
            {
                case "Перевод баллов":
                    _dataBase.AddMoney(rewardInfo.UserName, 5);
                    await _twitchApiClient.CompleteReward(rewardInfo.RewardId, rewardInfo.RedemptionId);
                    break;
            }
        }
    }
}