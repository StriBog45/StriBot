using Microsoft.Data.Sqlite;
using StriBot.Application.DataBase.Interfaces;

namespace StriBot.DateBase.Implementations
{
    public class DataBase : IDataBase
    {
        private const string BaseName = @"DateBase\StriBot.db3";
        private const string BasePath = "Data Source = " + BaseName;

        public void AddMoney(string nickname, int amount)
        {
            var clearName = CleanNickname(nickname);

            using var connection = new SqliteConnection(BasePath);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO Money ('nick', 'money', 'steam_trade_link') VALUES('{clearName}', 0, NULL) ON CONFLICT(nick) DO UPDATE SET money = money + {amount}";
            command.ExecuteNonQuery();
        }

        public int GetMoney(string nickname)
        {
            var clearName = CleanNickname(nickname);
            var money = 0;

            using var connection = new SqliteConnection(BasePath);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"SELECT money FROM Money WHERE nick = $nick";
            command.Parameters.AddWithValue("$nick", clearName);

            using var reader = command.ExecuteReader();
            reader.Read();
            if (int.TryParse(reader.GetString(0), out var result)) 
                money = result;

            return money;
        }

        public string CleanNickname(string nick)
            => nick[0] != '@' 
                ? nick 
                : nick.Remove(0, 1);

        public string GetSteamTradeLink(string nickname)
        {
            var clearName = CleanNickname(nickname);

            using var connection = new SqliteConnection(BasePath);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"SELECT steam_trade_link FROM Money WHERE nick = $nick";
            command.Parameters.AddWithValue("$nick", clearName);

            using var reader = command.ExecuteReader();
            reader.Read();

            return reader.GetString(0);
        }

        public void AddSteamTradeLink(string nickname, string steamTradeLink)
        {
            var clearName = CleanNickname(nickname);

            using var connection = new SqliteConnection(BasePath);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO Money ('nick', 'money', 'steam_trade_link') VALUES('{clearName}', 0, 'steamTradeLink') ON CONFLICT(nick) DO UPDATE SET steamTradeLink = '{steamTradeLink}'";
            command.ExecuteNonQuery();
        }
    }
}