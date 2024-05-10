using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainInitializer : MonoBehaviour
{
    private void Awake()
    {
        DataManager.Instance.OnDataManagerInit();
        //UIManager.Instance.accountListUI.RefreshAllUI();
        //UIManager.Instance.walletListUI.RefreshAllUI();
    }

    private void OnApplicationQuit()
    {
        DataManager.Instance.OnDataManagerQuit();
    }
}
