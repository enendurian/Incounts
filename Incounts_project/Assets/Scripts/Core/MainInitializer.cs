using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainInitializer : MonoBehaviour
{
    private void Awake()
    {
        //DataBaseInit();
        DataManager.Instance.OnDataManagerInit();
        UIManager.Instance.accountListUI.RefreshAllUI();
    }

    private void OnApplicationQuit()
    {
        DataManager.Instance.OnDataManagerQuit();
    }
}
