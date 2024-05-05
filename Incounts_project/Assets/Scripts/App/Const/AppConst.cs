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
        public const string aIsOut = "IsOut";   //0Ϊ���ˣ�1Ϊ����
        public const string aCount = "Count";
        public const string aType = "AccountType";
        public const string aIcon = "IconUrl";
        public const string aMessage = "Message";
        public const string aWalletId = "WalletId";

        public const string wIndex = "Id";
        public const string wName = "Name";
        public const string wBalance = "Balance";

        public const string intType = "INTEGER";
        public const string longType = "BIGINT";
        public const string textType = "TEXT";
    }

    public class BasicConsts
    {
        public static string[] WeekDays = { "������", "����һ", "���ڶ�", "������", "������", "������", "������" };
        public static string[] TypeNames = { "����", "��װ", "��ʳ", "�����", "��ͨ", "ҽ��", "����", "н��", "ת��" };
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
        //---------------����------------------
        Salary = 7,
        Transfer = 8,
    }

    public class Wallet
    {
        public string name;
        public long balance;
    }
}
