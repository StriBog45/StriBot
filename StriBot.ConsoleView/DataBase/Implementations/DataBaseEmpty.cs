using StriBot.Application.DataBase.Interfaces;

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
    }
}