using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainInitializer : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Initialize awake------------------");
        DataManager.Instance.OnDataManagerInit();
    }

    private void OnApplicationQuit()
    {
        DataManager.Instance.OnDataManagerQuit();
    }
}
