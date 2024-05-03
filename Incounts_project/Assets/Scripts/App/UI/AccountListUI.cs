using System.Collections;
using System.Collections.Generic;
using AppConst;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class AccountListUI : MonoBehaviour
{
    [Header("prefabs")]
    public GameObject itemDate;
    public GameObject itemAccounts;

    [Header("UIelements")]
    public RectTransform accountPanel;
    public RectTransform rectContent;
    public RectTransform rectPoolParent;
    public Text yearText;
    public Text monthText;
    public Text mainBalanceText;

    private List<SingleDayAccounts> currentShowingAccounts;     //日子的列表，便是月份了
    private int rectHeightCalculate;
    private readonly List<GameObject> objectShowing = new();

    private readonly Stack<GameObject> accountPool = new();
    private readonly Stack<GameObject> datePool = new();

    public void RefreshAllUI()
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
        currentShowingAccounts = DataManager.Instance.GetCurrentMonthAccounts();
        for (int i = 0; i < currentShowingAccounts.Count; i++)
        {
            ShowOneDay(currentShowingAccounts[i]);
        }
        rectContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectHeightCalculate);
        RefreshBalanceText();
    }

    /// <summary>
    /// 在最末尾增加新的记账
    /// </summary>
    public void AddSingleAccountOnLatest()
    {
        currentShowingAccounts = DataManager.Instance.GetCurrentMonthAccounts();
        AddSingleAccount(currentShowingAccounts.Last().accounts.Last());
        rectHeightCalculate += AccountListConst.Height_Account + AccountListConst.Height_Spacing;
        rectContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectHeightCalculate);
        RefreshBalanceText();
    }

    private void ResetRectHeightCalculate()
    {
        rectHeightCalculate = AccountListConst.Height_Blank;
    }

    private void ShowOneDay(SingleDayAccounts singleDay)
    {
        GameObject dateObject = GetFromPoolOrDefault(datePool, itemDate);
        dateObject.transform.SetParent(rectContent);
        rectHeightCalculate += AccountListConst.Height_Date + AccountListConst.Height_Spacing;
        dateObject.GetComponent<ItemDate>().RefreshDateData(singleDay);

        for (int i = 0; i < singleDay.accounts.Count; i++)
        {
            AddSingleAccount(singleDay.accounts[i]);
        }
    }

    private void AddSingleAccount(SingleAccount sa)
    {
        GameObject account = GetFromPoolOrDefault(accountPool, itemAccounts);
        account.transform.SetParent(rectContent);
        rectHeightCalculate += AccountListConst.Height_Account + AccountListConst.Height_Spacing;
        account.GetComponent<ItemAccount>().RefreshAccountData(sa);
    }

    public void RefreshYearAndMonth()
    {
        yearText.text = $"{DataManager.Instance.CurrentDataConfig.year}";
        monthText.text = DataManager.Instance.currentShowingMonth.ToString("D2");
    }

    public void RefreshBalanceText()
    {
        mainBalanceText.text = $"总余额：{DataManager.Instance.GetRemains()}";
    }

    #region Object pool
    void ClearShowingObjects()
    {
        foreach (GameObject go in objectShowing)
        {
            go.SetActive(false);
            go.transform.parent = rectPoolParent;
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
