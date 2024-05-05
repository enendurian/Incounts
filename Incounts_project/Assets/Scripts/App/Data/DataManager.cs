using System;
using UnityEngine;
using AppConst;
using Mono.Data.Sqlite;

public class DataManager
{
    private SqliteManager _sqliteManager;
    public int currentShowingMonth;
    public int currentShowingYear;

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
        DataBaseInit();
        CheckTableOfWallet();
        CheckTableOfMonth();
        //LoadRecordOfYear(currentShowingYear);
        /*if (_sqliteManager.TableExists("testTable01"))
        {
            Debug.Log("Table exists-----------------------tt01");
            _sqliteManager.InsertValues("testTable01", new string[] { "'1'", "'张三'", "'22'", "'Zhang3@163.com'" });
        }
        else
        {
            Debug.Log("Table do not exists--------------tt01");
            _sqliteManager.CreateTable("testTable01", new string[] { "ID", "Name", "Age", "Email" }, new string[] { "INTEGER", "TEXT", "INTEGER", "TEXT" });
        }*/
        /*_sqliteManager.InsertValues("testTable01", new string[] { "'3'", "'张9'", "'22'", "'Zhang3@163.com'" });
        int count = _sqliteManager.GetRecordCount("testTable01");
        Debug.Log($"Table record count {count}");*/
        //_sqliteManager.InsertValues(TConsts.walletTable, new string[] { "'11'", "'张9'", "'5000'" });
        //_sqliteManager.InsertValues(TConsts.walletTable, new string[] { "'12'", "'张10'", "'6000'" });
        //Debug.Log($"WalletRemains  : {GetWalletRemains()}");
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
                new string[] { TConsts.intType, TConsts.textType, TConsts.longType },
                TConsts.wIndex);
        }
    }

    private void CheckTableOfMonth()
    {
        if (!_sqliteManager.TableExists(currentDateTable))
        {
            _sqliteManager.CreateTable(currentDateTable,
                new string[] { TConsts.aIndex, TConsts.aTitle, TConsts.aDay, TConsts.aIsOut, TConsts.aCount, TConsts.aType, TConsts.aIcon, TConsts.aMessage, TConsts.aWalletId },
                new string[] { TConsts.intType, TConsts.textType, TConsts.intType, TConsts.intType, TConsts.intType, TConsts.intType, TConsts.textType, TConsts.textType, TConsts.intType },
                TConsts.aIndex);
        }
    }

    #region GetData
    //按照日期和主键排好序，遍历所有当前的记账记录
    public void TraverseAllRecords(Action<SqliteDataReader> action)
    {
        _sqliteManager.ExecuteQuery($"SELECT * FROM {currentDateTable} ORDER BY {TConsts.aDay} ASC, {TConsts.aIndex} ASC", action);
    }

    //读取钱包表，读取所有钱包余额总和
    public long GetWalletRemains()
    {
        _sqliteManager.ExecuteQuery($"SELECT SUM({TConsts.wBalance}) FROM {TConsts.walletTable}", out long result, reader =>
         {
             object sumObj = reader.GetValue(0);
             try
             {
                 return Convert.ToInt64(sumObj);
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
                itemAccount.typeText.text = reader.GetInt32(0).ToString();
                itemAccount.messageText.text = reader.GetString(1);
            }
        });
    }
    #endregion
}
