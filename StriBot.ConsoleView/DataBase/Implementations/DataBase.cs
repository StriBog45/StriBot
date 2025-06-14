﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using StriBot.Application.DataBase.Interfaces;
using StriBot.Application.DataBase.Models;

namespace StriBot.ConsoleView.DataBase.Implementations;

public class DataBase : IDataBase
{
    private const string DataBaseFilePath = @"StriBot.db3";
    private const string DataBaseConnectPath = "Data Source = " + DataBaseFilePath;
    //SQLiteConnection.CreateFile(baseName);

    private static readonly SQLiteFactory Factory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");

    public DataBase()
    {
        if (!File.Exists(DataBaseFilePath))
        {
            SQLiteConnection.CreateFile(DataBaseFilePath);
        }
    }

    public void AddMoney(string nickname, int amount)
    {
        var clearName = ClearNickname(nickname);

        var dTable = new DataTable();
        using var connection = (SQLiteConnection)Factory.CreateConnection();
        connection.ConnectionString = DataBaseConnectPath;
        connection.Open();
        var sqlCmd = new SQLiteCommand(connection);
        var amountBefore = 0;

        if (connection.State != ConnectionState.Open)
            throw new ArgumentException("Connection closed");

        try // чтение
        {
            var sqlQuery = $"SELECT nick, money FROM Money WHERE nick = '{clearName}'";
            var adapter = new SQLiteDataAdapter(sqlQuery, connection);
            adapter.Fill(dTable);

            if (dTable.Rows.Count > 0)
                amountBefore = Convert.ToInt32(dTable.Rows[0].ItemArray[1]);

            sqlCmd.CommandText =
                $"INSERT INTO Money ('nick', 'money') VALUES('{clearName}', '{amount}') ON CONFLICT(nick) DO UPDATE SET money = {amountBefore + amount};";
            sqlCmd.ExecuteNonQuery();
        }
        catch (SQLiteException exception)
        {
            throw exception;
        }
    }

    public int GetMoney(string nickname)
    {
        var clearName = ClearNickname(nickname);

        var dTable = new DataTable();
        using (var connection = (SQLiteConnection)Factory.CreateConnection())
        {
            connection.ConnectionString = DataBaseConnectPath;
            connection.Open();

            if (connection.State != ConnectionState.Open)
                throw new ArgumentException("Connection closed");
            try // чтение
            {
                var sqlQuery = $"SELECT nick, money FROM Money WHERE nick = '{clearName}'";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlQuery, connection);
                adapter.Fill(dTable);
            }
            catch (SQLiteException exception)
            {
                throw exception;
            }
        }

        return dTable.Rows.Count > 0
            ? Convert.ToInt32(dTable.Rows[0].ItemArray[1])
            : 0;
    }

    public string ClearNickname(string nick)
        => nick[0] != '@' 
            ? nick 
            : nick.Remove(0, 1);

    public string GetSteamTradeLink(string nickname)
    {
        var clearName = ClearNickname(nickname);

        var dTable = new DataTable();
        using (var connection = (SQLiteConnection)Factory.CreateConnection())
        {
            connection.ConnectionString = DataBaseConnectPath;
            connection.Open();

            if (connection.State != ConnectionState.Open)
                throw new ArgumentException("Connection closed");
            try // чтение
            {
                var sqlQuery = $"SELECT nick, steam_trade_link FROM Money WHERE nick = '{clearName}'";
                var adapter = new SQLiteDataAdapter(sqlQuery, connection);
                adapter.Fill(dTable);
            }
            catch (SQLiteException exception)
            {
                throw exception;
            }
        }

        return dTable.Rows.Count > 0 
            ? Convert.ToString(dTable.Rows[0].ItemArray[1]) 
            : null;
    }

    public void AddSteamTradeLink(string nickname, string steamTradeLink)
    {
        var clearName = ClearNickname(nickname);

        var dTable = new DataTable();
        using (var connection = (SQLiteConnection)Factory.CreateConnection())
        {
            connection.ConnectionString = DataBaseConnectPath;
            connection.Open();
            var sqlCmd = new SQLiteCommand(connection);
            var amountBefore = 0;

            if (connection.State != ConnectionState.Open)
                throw new ArgumentException("Connection closed");

            try // чтение
            {
                var sqlQuery = $"SELECT nick, money, steam_trade_link FROM Money WHERE nick = '{clearName}'";
                var adapter = new SQLiteDataAdapter(sqlQuery, connection);
                adapter.Fill(dTable);

                if (dTable.Rows.Count > 0)
                    amountBefore = Convert.ToInt32(dTable.Rows[0].ItemArray[1]);

                sqlCmd.CommandText = string.Format("INSERT INTO Money ('nick', 'money', 'steam_trade_link') VALUES('{0}', '{1}', '{2}') ON CONFLICT(nick) DO UPDATE SET steam_trade_link = '{2}';", clearName, amountBefore, steamTradeLink);
                sqlCmd.ExecuteNonQuery();
            }
            catch (SQLiteException exception)
            {
                throw exception;
            }
        }
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