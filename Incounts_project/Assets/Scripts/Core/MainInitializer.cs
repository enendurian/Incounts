using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainInitializer : MonoBehaviour
{
    private void Awake()
    {
        DataManager.OnInitDataManager();
    }
}
