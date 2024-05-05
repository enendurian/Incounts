using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalletListUI : MonoBehaviour
{
    [Header("prefabs")]
    public GameObject itemWallet;

    [Header("UIelements")]
    public RectTransform walletPanel;
    public RectTransform rectContent;
    public RectTransform rectPoolParent;
    public Text mainBalanceText;
}
