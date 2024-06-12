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

    private void Awake()
    {
        EventCenter.RegisterListener(AppConst.EventNamesConst.RefreshAccountList, RefreshAllUI);
    }

    public override void RefreshAllUI()
    {
        if (!isShowing)
        {
            isDirty = true;
            return;
        }
        RefreshYearAndMonth();
        RefreshListItems();
    }

    /// <summary>
    /// 完整地刷新显示的账目列表
    /// </summary>
    public void RefreshListItems()
    {
        Debug.Log("Run refresh list items--------------------------");
        ClearShowingObjects();
        ResetRectHeightCalculate();
        DataManager.Instance.TraverseAllRecords(ShowAllListItems);
        RefreshBalanceText();
    }

    private void ShowAllListItems(SqliteDataReader reader)
    {
        int lastday = -1;
        decimal temp_income = 0;
        decimal temp_outgo = 0;
        ItemDate temp_dateItem = null;
        emptyPanel.gameObject.SetActive(false);
        int tempDataCount = 0;
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
            decimal count = reader.GetDecimal(4);
            string walletName = DataManager.Instance.walletDataList[DataManager.Instance.WalletIndex2ListIndex(reader.GetInt32(8))].name;
            ShowNewAccount(reader.GetInt32(0), reader.GetString(1), isOut, count, walletName, reader.GetString(6));
            if (isOut <= 0)
                temp_outgo += count;
            else
                temp_income += count;
            tempDataCount++;
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

    private void ShowNewAccount(int pKey, string title, int isOut, decimal count, string walletName, string iconUrl)
    {
        GameObject account = GetFromPoolOrDefault(accountPool, itemAccounts);
        account.transform.SetParent(rectContent);
        rectHeightCalculate += AccountListConst.Height_Account + AccountListConst.Height_Spacing;
        account.GetComponent<ItemAccount>().RefreshAccountData(pKey, title, isOut, count, walletName, iconUrl);
        objectShowing.Add(account);
        account.SetActive(true);
    }

    public void MonthSwitchButtonClicked(int change)
    {
        DataManager.Instance.MonthChange(change);
        RefreshAllUI();
    }

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
        if (!isShowing)
            return;
        mainBalanceText.text = $"总余额：{DataManager.Instance.GetWalletRemains()}";
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(AppConst.EventNamesConst.RefreshAccountList, RefreshListItems);
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
        objectShowing.Clear();
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
