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

    public class BasicConsts
    {
        public static string[] WeekDays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
        public static string[] TypeNames = { "其他", "衣装", "饮食", "生活开销", "交通", "医疗", "娱乐", "薪酬", "转账" };
        public const string incomeColor = "#8FFFBE";
        public const string outgoColor = "#FFAAAA";
        public AccountTypes[] outTypes = { AccountTypes.Others, AccountTypes.Clothes, AccountTypes.Food, AccountTypes.Lives, AccountTypes.Traffic, AccountTypes.Medic, AccountTypes.Entertain };
        public AccountTypes[] inTypes = { AccountTypes.Others, AccountTypes.Salary, AccountTypes.Transfer };
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
