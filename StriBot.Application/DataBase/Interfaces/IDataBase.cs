using System.Collections.Generic;
using StriBot.Application.DataBase.Models;

namespace StriBot.Application.DataBase.Interfaces
{
    public interface IDataBase
    {
        void AddMoney(string nickname, int amount);

        int GetMoney(string nickname);

        string CleanNickname(string nick);

        string GetSteamTradeLink(string nickname);

        void AddSteamTradeLink(string nickname, string steamTradeLink);

        int GetBananaSize(string nickname);

        void IncreaseBananaSize(string nickname);

        void SetBananaSize(string nickname, int bananaSize);

        List<BananaInfo> GetTopBananas();

        int GetFirstViewerTimes(string nickname);

        void IncreaseFirstViewerTimes(string nickname);
    }
}