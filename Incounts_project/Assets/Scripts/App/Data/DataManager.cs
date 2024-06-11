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
                instance.OnDataManagerInit();
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
        RefreshWalletData();
        CheckTableOfMonth();
        EventCenter.RegisterListener(AppConst.EventNamesConst.RefreshWalletData, RefreshWalletData);
    }
    public void MonthChange(int change)
    {
        currentShowingMonth += change;
        if (currentShowingMonth <= 0)
        {
            currentShowingMonth = 12;
            currentShowingYear -= 1;
        }
        if (currentShowingMonth > 12)
        {
            currentShowingMonth = 1;
            currentShowingYear += 1;
        }
        CheckTableOfMonth();
    }

    public void OnDataManagerQuit()
    {
        _sqliteManager.Dispose();
    }

    void DataBaseInit()
    {
        string path = Application.persistentDataPath + "/database/incounts.db";
        Debug.Log($"The Path is {path}");
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
                decimal balance = reader.GetDecimal(2);
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

    public int WalletIndex2ListIndex(int walletIndex)
    {
        for (int i = 0; i < walletDataList.Count; i++)
        {
            if (walletDataList[i].index == walletIndex)
                return i;
        }
        return -1;
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
        _sqliteManager.ExecuteQuery($"SELECT * FROM {TConsts.walletTable} ORDER BY {TConsts.wBalance} DESC", action);
    }

    //读取钱包表，读取所有钱包余额总和
    public decimal GetWalletRemains()
    {
        _sqliteManager.ExecuteQuery($"SELECT SUM({TConsts.wBalance}) FROM {TConsts.walletTable}", out decimal result, reader =>
         {
             object sumObj = reader.GetValue(0);
             try
             {
                 return Convert.ToDecimal(sumObj);
             }
             catch (InvalidCastException)
             {
                 return 0;
             }
         });
        return result;
    }

    //读取钱包表，读取有几个钱包
    public int GetWalletCount()
    {
        _sqliteManager.ExecuteQuery($"SELECT COUNT(*) FROM {TConsts.walletTable}", out int result, reader =>
        {
            object sumObj = reader.GetValue(0);
            try
            {
                return Convert.ToInt32(sumObj);
            }
            catch (InvalidCastException)
            {
                return 0;
            }
        });
        return result;
    }

    public void ShowDetailsOfAccount(ItemAccount itemAccount, int pkey)
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

    public void ShowDetailsOfAccount(int pkey, Action<SqliteDataReader> showAction)
    {
        string command = $"SELECT * FROM {currentDateTable} WHERE {TConsts.aIndex} = {pkey}";
        _sqliteManager.ExecuteQuery(command, reader =>
        {
            if (reader.Read())
            {
                showAction.Invoke(reader);
            }
        });
    }

    public void ShowDetailsOfWallet(int pkey, Action<SqliteDataReader> showAction)
    {
        string command = $"SELECT * FROM {TConsts.walletTable} WHERE {TConsts.wIndex} = {pkey}";
        _sqliteManager.ExecuteQuery(command, reader =>
        {
            if (reader.Read())
            {
                showAction.Invoke(reader);
            }
        });
    }
    #endregion

    #region Insert Data
    //在当前月份的表中插入记账记录。这里的方法只执行sql语句和数据库管理。一些界面更新或者数据同步，请不要写在这里。
    public void AddAccount(string title, int day, int isOut, decimal count, int accountType, string iconId, string message, int walletId)
    {
        int index = _sqliteManager.GetMaxInt(currentDateTable, TConsts.aIndex);
        index++;
        string[] values = new string[] { $"{index}", $"'{title}'", $"{day}", $"{isOut}", $"{count}", $"{accountType}", $"'{iconId}'", $"'{message}'", $"{walletId}" };
        _sqliteManager.InsertValues(currentDateTable, values);
    }

    public void AddWallet(string name, decimal startBalance)
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

    /*public void DeleteWallet(int index)
    {
        _sqliteManager.DeleteValuesAND(TConsts.walletTable, new string[] { TConsts.wIndex }, new string[] { "=" }, new string[] { $"'{index}'" });
    }*/
    #endregion

    #region Edit Data
    public void UpdateAccount(string title, int day, int isOut, decimal count, int accountType, string iconId, string message, int walletId, int pkey)
    {
        string[] colNames = new string[] { TConsts.aTitle, TConsts.aDay, TConsts.aIsOut, TConsts.aCount, TConsts.aType, TConsts.aIcon, TConsts.aMessage, TConsts.aWalletId };
        string[] values = new string[] { $"'{title}'", $"{day}", $"{isOut}", $"{count}", $"{accountType}", $"'{iconId}'", $"'{message}'", $"{walletId}" };
        _sqliteManager.UpdateValues(currentDateTable, colNames, values, TConsts.aIndex, "=", $"{pkey}");
    }

    public void UpdateWallet(string name, decimal startBalance, int pkey)
    {
        string[] colNames = new string[] { TConsts.wName, TConsts.wBalance };
        string[] values = new string[] { $"'{name}'", $"{startBalance}" };
        _sqliteManager.UpdateValues(TConsts.walletTable, colNames, values, TConsts.wIndex, "=", $"{pkey}");
    }

    public void UpdateWallet(decimal changeBalance, int pkey)
    {
        decimal resultBalance = walletDataList[WalletIndex2ListIndex(pkey)].balance + changeBalance;
        string[] colNames = new string[] { TConsts.wBalance };
        string[] values = new string[] { $"{resultBalance}" };
        _sqliteManager.UpdateValues(TConsts.walletTable, colNames, values, TConsts.wIndex, "=", $"{pkey}");
    }
    #endregion
}

public class WalletDataItem
{
    public int index;
    public string name;
    public decimal balance;

    public void Update(int idx, string nm, decimal bl)
    {
        index = idx;
        name = nm;
        balance = bl;
    }
}