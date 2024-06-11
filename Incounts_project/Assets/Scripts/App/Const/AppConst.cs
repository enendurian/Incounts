using System.Collections.Generic;

namespace AppConst
{
    public class AccountListConst
    {
        public const int Height_Account = 150;
        public const int Height_Date = 120;
        public const int Height_Spacing = 10;
        public const int Height_Blank = 300;
    }

    public class WalletListConst
    {
        public const int Height_Wallet = 200;
        public const int Height_Spacing = 10;
        public const int Height_Blank = 400;
    }

    public class TConsts
    {
        public const string accountTable = "accountTable";
        public const string walletTable = "walletTable";

        public const string aIndex = "Id";
        public const string aTitle = "Title";
        public const string aDay = "Day";
        public const string aIsOut = "IsOut";   //0为出账，1为入账
        public const string aCount = "Count";
        public const string aType = "AccountType";
        public const string aIcon = "IconId";
        public const string aMessage = "Message";
        public const string aWalletId = "WalletId";

        public const string wIndex = "Id";
        public const string wName = "Name";
        public const string wBalance = "Balance";

        public const string intType = "INTEGER";
        public const string longType = "BIGINT";
        public const string floatType = "REAL";
        public const string textType = "TEXT";
    }

    public class BasicConsts
    {
        public static string[] WeekDays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
        public static string[] TypeNames = { "其他", "衣装", "饮食", "生活开销", "交通", "医疗", "娱乐", "薪酬", "转账" };
        public const string incomeColor = "#8FFFBE";
        public const string outgoColor = "#FFAAAA";
        public const string OutAccount = "支出";
        public const string InAccount = "收入";
        public static AccountTypes[] outTypes = { AccountTypes.Others, AccountTypes.Clothes, AccountTypes.Food, AccountTypes.Lives, AccountTypes.Traffic, AccountTypes.Medic, AccountTypes.Entertain };
        public static AccountTypes[] inTypes = { AccountTypes.Others, AccountTypes.Salary, AccountTypes.Transfer };
        public const string addressIcon = "Assets/Res/images/{0}.png";
        public static Dictionary<int, List<string>> iconListDict = new()
        {
            { 0,new List<string>{ "0_0" } },
            { 1,new List<string>{ "1_0"} },
            { 2,new List<string>{ "2_0"} },
            { 3,new List<string>{ "3_0"} },
            { 4,new List<string>{ "4_0"} },
            { 5,new List<string>{ "5_0"} },
            { 6,new List<string>{ "6_0"} },
            { 7,new List<string>{ "7_0"} },
            { 8,new List<string>{ "8_0"} },
        };
    }

    public enum AccountTypes
    {
        Others = 0,
        Clothes = 1,
        Food = 2,
        Lives = 3,
        Traffic = 4,
        Medic = 5,
        Entertain = 6,
        //---------------入账------------------
        Salary = 7,
        Transfer = 8,
    }

    public class Wallet
    {
        public string name;
        public long balance;
    }
}
