using System.Collections.Generic;
using AppConst;

//ÿ��һ��������Config
public class AccountDataConfig
{
    //��Ŀ
    public int year;
    public Dictionary<int, List<SingleDayAccounts>> accountsInMonths;

    //Ǯ��
    public List<Wallet> wallets;

    public List<SingleDayAccounts> GetAccountOfMonth(int month)
    {
        if (!accountsInMonths.ContainsKey(month))
        {
            List<SingleDayAccounts> accountsOfDay = new();
            accountsInMonths[month] = accountsOfDay;
        }
        return accountsInMonths[month];
    }
}

public class SingleDayAccounts
{
    public int date;
    public int dayOfweek;
    public List<SingleAccount> accounts = new();

    public (int income, int outgo) SumOfIncoming()
    {
        int income = 0;
        int outgo = 0;
        for (int i = 0; i < accounts.Count; i++)
        {
            if (accounts[i].isOut)
            {
                outgo += accounts[i].count;
            }
            else
            {
                income += accounts[i].count;
            }
        }
        return (income, outgo);
    }
}

public class SingleAccount
{
    public string title;
    public bool isOut;  //�Ƿ�Ϊ���ˣ���trueΪ���ˣ�falseΪ����
    public int count;
    public AccountTypes type;
    public string iconURL;
    public string message;
    public int index;   //�ڵ��յ�����
}