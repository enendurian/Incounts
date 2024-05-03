using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    public AccountDataConfig CurrentDataConfig => _currentDataConfig;
    private AccountDataConfig _currentDataConfig;
    public int currentShowingMonth;

    public static DataManager Instance;

    public static void OnInitDataManager()
    {
        if (Instance != null)
            return;
        Instance = new DataManager();
        Instance.ReadDataFromLocal();
    }

    void ReadDataFromLocal()
    {
        /*_currentDataConfig = new AccountDataConfig()
        {
            year = 2023
        };
        currentShowingMonth = 4;*/
    }

    public List<SingleDayAccounts> GetCurrentMonthAccounts()
    {
        return _currentDataConfig.GetAccountOfMonth(currentShowingMonth);
    }

    public long GetRemains()
    {
        long result = 0;
        for (int i = 0; i < _currentDataConfig.wallets.Count; i++)
        {
            result += _currentDataConfig.wallets[i].balance;
        }
        return result;
    }
}
