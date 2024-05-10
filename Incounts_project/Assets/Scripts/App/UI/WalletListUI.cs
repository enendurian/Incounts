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

    public override void RefreshAllUI()
    {
        RefreshBalance();
        RefreshListItems();
    }

    public void RefreshBalance()
    {
        mainBalanceText.text = DataManager.Instance.GetWalletRemains().ToString();
    }

    public void RefreshListItems()
    {
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
            long balance = reader.GetInt64(2);
            ShowNewWalllet(pkey, name, balance);
        }
        emptyPanel.gameObject.SetActive(objectShowing.Count <= 0);
        rectContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectHeightCalculate);
    }

    void ShowNewWalllet(int pkey, string name, long balance)
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
