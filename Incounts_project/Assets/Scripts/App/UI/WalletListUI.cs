using System.Collections;
using System.Collections.Generic;
using AppConst;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class WalletListUI : UIPagesBase
{
    [Header("prefabs")]
    public GameObject itemWallet;

    [Header("UIelements")]
    public RectTransform walletPanel;
    public RectTransform rectContent;
    public RectTransform rectPoolParent;
    public RectTransform emptyPanel;
    public Text mainBalanceText;

    private int rectHeightCalculate;
    private readonly List<GameObject> objectShowing = new();
    private readonly Stack<GameObject> walletPool = new();

    private void Awake()
    {
        EventCenter.RegisterListener(AppConst.EventNamesConst.RefreshWalletList, RefreshAllUI);
    }

    public override void RefreshAllUI()
    {
        if (!isShowing)
        {
            isDirty = true;
            return;
        }
        RefreshBalance();
        RefreshListItems();
    }

    public void RefreshBalance()
    {
        mainBalanceText.text = DataManager.Instance.GetWalletRemains().ToString();
    }

    public void RefreshListItems()
    {
        Debug.Log("Run refresh list items--------------------------");
        ClearShowingObjects();
        ResetRectHeightCalculate();
        DataManager.Instance.TraverseAllWallets(ShowAllListItems);
    }

    private void ShowAllListItems(SqliteDataReader reader)
    {
        while (reader.Read())
        {
            int pkey = reader.GetInt32(0);
            string name = reader.GetString(1);
            decimal balance = reader.GetDecimal(2);
            ShowNewWalllet(pkey, name, balance);
        }
        emptyPanel.gameObject.SetActive(objectShowing.Count <= 0);
        rectContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectHeightCalculate);
    }

    void ShowNewWalllet(int pkey, string name, decimal balance)
    {
        GameObject wallet = GetFromPoolOrDefault(walletPool, itemWallet);
        wallet.transform.SetParent(rectContent);
        rectHeightCalculate += WalletListConst.Height_Wallet + WalletListConst.Height_Spacing;
        wallet.GetComponent<ItemWallet>().RefreshWalletData(pkey, name,balance);
        objectShowing.Add(wallet);
        wallet.SetActive(true);
    }

    void ResetRectHeightCalculate()
    {
        rectHeightCalculate = WalletListConst.Height_Blank;
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(AppConst.EventNamesConst.RefreshWalletList, RefreshAllUI);
    }

    #region Object pool
    void ClearShowingObjects()
    {
        foreach (GameObject go in objectShowing)
        {
            go.SetActive(false);
            go.transform.SetParent(rectPoolParent);
            go.GetComponent<ItemWallet>().HardReset();
            walletPool.Push(go);
        }
        objectShowing.Clear();
    }

    GameObject GetFromPoolOrDefault(Stack<GameObject> pool, GameObject defaultGo)
    {
        if (pool.Count > 0)
        {
            return pool.Pop();
        }
        return Instantiate(defaultGo, defaultGo.transform.parent);
    }
    #endregion
}
