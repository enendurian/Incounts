using System.Collections;
using AppConst;
using UnityEngine;
using UnityEngine.UI;

public class ItemDate : MonoBehaviour
{
    [Header("UIElements")]
    public Text dateText;
    public Text sumAccountText;

    private SingleDayAccounts ad;

    public void RefreshDateData(SingleDayAccounts accountData)
    {
        ad = accountData;
        dateText.text = $"{ad.date} {BasicConsts.WeekDays[ad.dayOfweek]}";
        (int income, int outgo) = ad.SumOfIncoming();
        sumAccountText.text = $"<color={BasicConsts.incomeColor}>+{income}</color> <color={BasicConsts.outgoColor}>-{outgo}</color>";
    }
}
