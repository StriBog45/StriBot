using System;
using System.Data;
using System.Data.SQLite;
using System.Data.Common;



namespace StriBot
{
    static public class DataBase
    {
        static string baseName = @"DateBase\StriBot.db3";
        static string basePath = "Data Source = " + baseName;
        static SQLiteCommand sqlCmd = new SQLiteCommand();
        //SQLiteConnection.CreateFile(baseName);

        static SQLiteFactory factory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");

        public static void AddMoneyToUser(string nickname, int amount)
        {
            var clearName = CleanNickname(nickname);

            DataTable dTable = new DataTable();
            using (SQLiteConnection connection = (SQLiteConnection)factory.CreateConnection())
            {
                connection.ConnectionString = basePath;
                connection.Open();
                sqlCmd.Connection = connection;
                int amountBefore = 0;
                if (connection.State != ConnectionState.Open)
                    throw new ArgumentException("Connection closed");
                try // чтение
                {
                    var sqlQuery = String.Format("SELECT * FROM Money WHERE nick = '{0}'", clearName);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlQuery, connection);
                    adapter.Fill(dTable);

                    if (dTable.Rows.Count > 0)
                        amountBefore = Convert.ToInt32(dTable.Rows[0].ItemArray[1]);

                    sqlCmd.CommandText = String.Format("INSERT INTO Money ('nick', 'money') VALUES('{0}', '{1}') ON CONFLICT(nick) DO UPDATE SET money = {2};", clearName, amount, amountBefore+amount);
                    sqlCmd.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    throw ex;
                }
            }
        }

        public static int CheckMoney(string nickname)
        {
            var clearName = CleanNickname(nickname);

            DataTable dTable = new DataTable();
            using (SQLiteConnection connection = (SQLiteConnection)factory.CreateConnection())
            {
                connection.ConnectionString = basePath;
                connection.Open();
                sqlCmd.Connection = connection;

                if (connection.State != ConnectionState.Open)
                    throw new ArgumentException("Connection closed");
                try // чтение
                {
                    var sqlQuery = String.Format("SELECT * FROM Money WHERE nick = '{0}'", clearName);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlQuery, connection);         
                    adapter.Fill(dTable);
                }
                catch (SQLiteException ex)
                {
                    throw ex;
                }
            }
            if (dTable.Rows.Count > 0)
                return Convert.ToInt32(dTable.Rows[0].ItemArray[1]);
            else
                return 0;
        }

        public static string CleanNickname(string nick)
            => nick[0] != '@' ? nick : nick.Remove(0, 1);
    }
}
