using System.Collections.Generic;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.DataBase.Models;

namespace StriBot.ConsoleView.DataBase.Implementations
{
    public class DataBaseEmpty : IDataBase
    {
        public void AddMoney(string nickname, int amount)
        {
        }

        public int GetMoney(string nickname)
            => 0;

        public string CleanNickname(string nick)
            => "StriBot";

        public string GetSteamTradeLink(string nickname)
            => string.Empty;

        public void AddSteamTradeLink(string nickname, string steamTradeLink)
        {
        }

        public int GetBananaSize(string nickname)
            => 0;

        public void IncreaseBananaSize(string nickname)
        {
        }

        public void SetBananaSize(string nickname, int bananaSize)
        {
        }

        public List<BananaInfo> GetTopBananas()
            => new();

        public int GetFirstViewerTimes(string nickname)
            => 0;

        public void IncreaseFirstViewerTimes(string nickname)
        {
        }
    }
}