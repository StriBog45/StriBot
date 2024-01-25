using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.DataBase.Models;

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

            return reader.Read()
                ? reader.IsDBNull(0)
                    ? null
                    : reader.GetString(0)
                : null;
        }

        public void AddSteamTradeLink(string nickname, string steamTradeLink)
        {
            var clearName = CleanNickname(nickname);

            using var connection = new SqliteConnection(BasePath);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO Money ('nick', 'money', 'steam_trade_link') VALUES('{clearName}', 0, 'steamTradeLink') ON CONFLICT(nick) DO UPDATE SET steam_trade_link = '{steamTradeLink}'";
            command.ExecuteNonQuery();
        }

        public int GetBananaSize(string nickname)
        {
            var clearName = CleanNickname(nickname);
            var bananaSize = 0;

            using var connection = new SqliteConnection(BasePath);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"SELECT banana_size FROM Money WHERE nick = $nick";
            command.Parameters.AddWithValue("$nick", clearName);

            using var reader = command.ExecuteReader();
            reader.Read();
            if (int.TryParse(reader.GetString(0), out var result)) 
                bananaSize = result;

            return bananaSize;
        }

        public void IncreaseBananaSize(string nickname)
        {
            var clearName = CleanNickname(nickname);

            using var connection = new SqliteConnection(BasePath);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"UPDATE Money SET banana_size = banana_size + 1 WHERE nick = $nick";
            command.Parameters.AddWithValue("$nick", clearName);
            command.ExecuteNonQuery();
        }

        public List<BananaInfo> GetTopBananas()
        {
            var result = new List<BananaInfo>();
            using var connection = new SqliteConnection(BasePath);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"SELECT nick, banana_size FROM Money ORDER BY banana_size DESC LIMIT 3";
            command.ExecuteScalar();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var nick = reader.GetString(reader.GetOrdinal("nick"));
                var bananaSize = reader.GetInt32(reader.GetOrdinal("banana_size"));

                result.Add(new BananaInfo
                {
                    Nick = nick,
                    BananaSize = bananaSize
                });
            }

            return result;
        }

        public int GetFirstViewerTimes(string nickname)
        {
            var clearName = CleanNickname(nickname);
            var bananaSize = 0;

            using var connection = new SqliteConnection(BasePath);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"SELECT first_viewer_times FROM Money WHERE nick = $nick";
            command.Parameters.AddWithValue("$nick", clearName);

            using var reader = command.ExecuteReader();
            reader.Read();
            if (int.TryParse(reader.GetString(0), out var result)) 
                bananaSize = result;

            return bananaSize;
        }

        public void IncreaseFirstViewerTimes(string nickname)
        {
            var clearName = CleanNickname(nickname);

            using var connection = new SqliteConnection(BasePath);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"UPDATE Money SET first_viewer_times = COALESCE(first_viewer_times, 0) + 1 WHERE nick = $nick";
            command.Parameters.AddWithValue("$nick", clearName);
            command.ExecuteNonQuery();
        }
    }
}