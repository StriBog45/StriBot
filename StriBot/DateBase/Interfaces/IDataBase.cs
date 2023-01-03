namespace StriBot.DateBase.Interfaces;

public interface IDataBase
{
    void AddMoney(string nickname, int amount);

    int GetMoney(string nickname);

    string CleanNickname(string nick);

    string GetSteamTradeLink(string nickname);

    void AddSteamTradeLink(string nickname, string steamTradeLink);
}