using System;
using UnityEngine;
using AppConst;
using Mono.Data.Sqlite;
using System.Collections.Generic;

public class DataManager
{
    private SqliteManager _sqliteManager;
    public int currentShowingMonth;
    public int currentShowingYear;
    public int today;
    public List<WalletDataItem> walletDataList = new List<WalletDataItem>();

    private static DataManager instance;

    private string currentDateTable => $"{TConsts.accountTable}_{currentShowingYear}_{currentShowingMonth}";

    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DataManager();
            }
            return instance;
        }
    }

    public void OnDataManagerInit()
    {
        DateTime now = DateTime.Now;
        currentShowingYear = now.Year;
        currentShowingMonth = now.Month;
        today = now.Day;
        DataBaseInit();
        CheckTableOfWallet();
        CheckTableOfMonth();
        RefreshWalletData();
    }

    public void OnDataManagerQuit()
    {
        _sqliteManager.Dispose();
    }

    void DataBaseInit()
    {
        string path = Application.dataPath + "/database/test.db";
        _sqliteManager = new(path);
    }

    private void CheckTableOfWallet()
    {
        if (!_sqliteManager.TableExists(TConsts.walletTable))
        {
            _sqliteManager.CreateTable(TConsts.walletTable,
                new string[] { TConsts.wIndex, TConsts.wName, TConsts.wBalance },
                new string[] { TConsts.intType, TConsts.textType, TConsts.floatType },
                TConsts.wIndex);
        }
    }

    private void CheckTableOfMonth()
    {
        if (!_sqliteManager.TableExists(currentDateTable))
        {
            _sqliteManager.CreateTable(currentDateTable,
                new string[] { TConsts.aIndex, TConsts.aTitle, TConsts.aDay, TConsts.aIsOut, TConsts.aCount, TConsts.aType, TConsts.aIcon, TConsts.aMessage, TConsts.aWalletId },
                new string[] { TConsts.intType, TConsts.textType, TConsts.intType, TConsts.intType, TConsts.floatType, TConsts.intType, TConsts.textType, TConsts.textType, TConsts.intType },
                TConsts.aIndex);
        }
    }

    public void RefreshWalletData()
    {
        TraverseAllWallets((reader) =>
        {
            int count = 0;
            while (reader.Read())
            {
                int pkey = reader.GetInt32(0);
                string name = reader.GetString(1);
                float balance = reader.GetFloat(2);
                if (count < walletDataList.Count)
                {
                    walletDataList[count].Update(pkey, name, balance);
                }
                else
                {
                    walletDataList.Add(new WalletDataItem() { index = pkey, name = name, balance = balance });
                }
                count++;
            }
        });
    }

    #region GetData
    //按照日期和主键排好序，遍历所有当前的记账记录
    public void TraverseAllRecords(Action<SqliteDataReader> action)
    {
        _sqliteManager.ExecuteQuery($"SELECT * FROM {currentDateTable} ORDER BY {TConsts.aDay} ASC, {TConsts.aIndex} ASC", action);
    }

    //按照主键排好序，遍历所有当前的钱包
    public void TraverseAllWallets(Action<SqliteDataReader> action)
    {
        _sqliteManager.ExecuteQuery($"SELECT * FROM {TConsts.walletTable} ORDER BY {TConsts.wIndex} ASC", action);
    }

    //读取钱包表，读取所有钱包余额总和
    public double GetWalletRemains()
    {
        _sqliteManager.ExecuteQuery($"SELECT SUM({TConsts.wBalance}) FROM {TConsts.walletTable}", out double result, reader =>
         {
             object sumObj = reader.GetValue(0);
             try
             {
                 return Convert.ToDouble(sumObj);
             }
             catch (InvalidCastException)
             {
                 return 0;
             }
         });
        return result;
    }

    public void ShowDetailsOfAccount(ItemAccount itemAccount,int pkey)
    {
        string command = $"SELECT {TConsts.aType}, {TConsts.aMessage} FROM {currentDateTable} WHERE {TConsts.aIndex} = {pkey}";
        _sqliteManager.ExecuteQuery(command, reader =>
        {
            if (reader.Read())
            {
                itemAccount.typeText.text = BasicConsts.TypeNames[reader.GetInt32(0)];
                itemAccount.messageText.text = reader.GetString(1);
            }
        });
    }
    #endregion

    #region Insert Data
    //在当前月份的表中插入记账记录。这里的方法只执行sql语句和数据库管理。一些界面更新或者数据同步，请不要写在这里。
    public void AddAccount(string title, int day, int isOut, float count, int accountType, string iconId, string message, int walletId)
    {
        int index = _sqliteManager.GetMaxInt(currentDateTable, TConsts.aIndex);
        index++;
        string[] values = new string[] { $"{index}", $"'{title}'", $"{day}", $"{isOut}", $"{count}", $"{accountType}", $"'{iconId}'", $"'{message}'", $"{walletId}" };
        _sqliteManager.InsertValues(currentDateTable, values);
    }

    public void AddWallet(string name, float startBalance)
    {
        int index = _sqliteManager.GetMaxInt(TConsts.walletTable, TConsts.wIndex);
        index++;
        string[] values = new string[] { $"{index}", $"'{name}'", $"{startBalance}" };
        _sqliteManager.InsertValues(TConsts.walletTable, values);
    }
    #endregion

    #region Drop Data
    public void DeleteAccount(int index)
    {
        _sqliteManager.DeleteValuesAND(currentDateTable, new string[] { TConsts.aIndex }, new string[] { "=" }, new string[] { $"'{index}'" });
    }

    public void DeleteWallet(int index)
    {
        _sqliteManager.DeleteValuesAND(TConsts.walletTable, new string[] { TConsts.wIndex }, new string[] { "=" }, new string[] { $"'{index}'" });
    }
    #endregion
}

public class WalletDataItem
{
    public int index;
    public string name;
    public double balance;

    public void Update(int idx, string nm, double bl)
    {
        index = idx;
        name = nm;
        balance = bl;
    }
}