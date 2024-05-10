using System.Collections.Generic;
using AppConst;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System;

public class AccountListUI : UIPagesBase
{
    [Header("prefabs")]
    public GameObject itemDate;
    public GameObject itemAccounts;

    [Header("UIelements")]
    public RectTransform rectContent;
    public RectTransform rectPoolParent;
    public RectTransform emptyPanel;
    public Text yearText;
    public Text monthText;
    public Text mainBalanceText;

    private int rectHeightCalculate;
    private readonly List<GameObject> objectShowing = new();

    private readonly Stack<GameObject> accountPool = new();
    private readonly Stack<GameObject> datePool = new();

    public override void RefreshAllUI()
    {
        RefreshYearAndMonth();
        RefreshListItems();
    }

    /// <summary>
    /// 完整地刷新显示的账目列表
    /// </summary>
    public void RefreshListItems()
    {
        ClearShowingObjects();
        ResetRectHeightCalculate();
        DataManager.Instance.TraverseAllRecords(ShowAllListItems);
        RefreshBalanceText();
    }

    private void ShowAllListItems(SqliteDataReader reader)
    {
        int lastday = -1;
        int temp_income = 0;
        int temp_outgo = 0;
        ItemDate temp_dateItem = null;
        emptyPanel.gameObject.SetActive(false);

        while (reader.Read())
        {
            int dayRead = reader.GetInt32(2);
            //是新日期，先增加横幅
            if (lastday < dayRead)
            {
                lastday = dayRead;
                if (temp_dateItem != null)
                {
                    temp_dateItem.sumAccountText.text = $"<color={BasicConsts.incomeColor}>+{temp_income}</color> <color={BasicConsts.outgoColor}>-{temp_outgo}</color>";
                }
                temp_dateItem = ShowNewDay(dayRead);
                temp_income = 0;
                temp_outgo = 0;
            }
            //增加账目
            int isOut = reader.GetInt32(3);
            int count = reader.GetInt32(4);
            ShowNewAccount(reader.GetInt32(0), reader.GetString(1), isOut, count, reader.GetString(6));
            if (isOut <= 0)
                temp_outgo += count;
            else
                temp_income += count;
        }

        //再刷新一下最后一天的总收支数据
        if (temp_dateItem != null)
        {
            temp_dateItem.sumAccountText.text = $"<color={BasicConsts.incomeColor}>+{temp_income}</color> <color={BasicConsts.outgoColor}>-{temp_outgo}</color>";
        }
        else
        {
            //没有最后一天的数据说明一天的数据都没有
            emptyPanel.gameObject.SetActive(true);
        }
        rectContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectHeightCalculate);
    }

    private ItemDate ShowNewDay(int day)
    {
        GameObject dateObject = GetFromPoolOrDefault(datePool, itemDate);
        dateObject.transform.SetParent(rectContent);
        rectHeightCalculate += AccountListConst.Height_Date + AccountListConst.Height_Spacing;
        ItemDate idate = dateObject.GetComponent<ItemDate>();
        DateTime date = new(DataManager.Instance.currentShowingYear, DataManager.Instance.currentShowingMonth, day);
        DayOfWeek dayOfWeek = date.DayOfWeek;
        idate.dateText.text = $"{day} {BasicConsts.WeekDays[(int)dayOfWeek]}";
        objectShowing.Add(dateObject);
        dateObject.SetActive(true);
        return idate;
    }

    private void ShowNewAccount(int pKey, string title, int isOut, int count, string iconUrl)
    {
        GameObject account = GetFromPoolOrDefault(accountPool, itemAccounts);
        account.transform.SetParent(rectContent);
        rectHeightCalculate += AccountListConst.Height_Account + AccountListConst.Height_Spacing;
        account.GetComponent<ItemAccount>().RefreshAccountData(pKey, title, isOut, count, iconUrl);
        objectShowing.Add(account);
        account.SetActive(true);
    }

    /// <summary>
    /// 在最末尾增加新的记账
    /// </summary>
    /*public void AddSingleAccountOnLatest()
    {
        //currentShowingAccounts = DataManager.Instance.GetCurrentMonthAccounts();
        //AddSingleAccount(currentShowingAccounts.Last().accounts.Last());
        rectHeightCalculate += AccountListConst.Height_Account + AccountListConst.Height_Spacing;
        rectContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectHeightCalculate);
        RefreshBalanceText();
    }*/

    private void ResetRectHeightCalculate()
    {
        rectHeightCalculate = AccountListConst.Height_Blank;
    }

    public void RefreshYearAndMonth()
    {
        yearText.text = $"{DataManager.Instance.currentShowingYear}";
        monthText.text = DataManager.Instance.currentShowingMonth.ToString("D2");
    }

    public void RefreshBalanceText()
    {
        mainBalanceText.text = $"总余额：{DataManager.Instance.GetWalletRemains()}";
    }

    #region Object pool
    void ClearShowingObjects()
    {
        foreach (GameObject go in objectShowing)
        {
            go.SetActive(false);
            go.transform.SetParent(rectPoolParent);
            if (go.TryGetComponent(out ItemAccount ia))
            {
                ia.HardReset();
                accountPool.Push(go);
                continue;
            }
            if (go.TryGetComponent(out ItemDate id))
            {
                datePool.Push(go);
                continue;
            }
        }
    }

    GameObject GetFromPoolOrDefault(Stack<GameObject> pool,GameObject defaultGo)
    {
        if (pool.Count > 0)
        {
            return pool.Pop();
        }
        return Instantiate(defaultGo, defaultGo.transform.parent);
    }
    #endregion
}
