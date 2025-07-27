using Shared.Roles;

namespace Shared.Enums
{
    public enum CustomerStatus
    {
        Debtor = 1,    // بدهکار
        Creditor = 2   // بستانکار
    }

    public enum eAccountType
    {
        Regular,
        Treasury,
        Incremental,
        Decremental,
        CurrencyExchange
    }

    public enum Currency
    {
        Dollar,
        Euro,
        Afghani,
        Kaldar,
        Toman,
    }

    // نوعیت معامله
    public enum DealType
    {
        Deposit,
        Withdraw
    }

    // نوعیت کمشن
    public enum CommisionType
    {
        Cash,
        FromSender,
        FromReciever,
        NoComission,
    }

    public enum TransactionType
    {
        Normal,// Only deposit or withdraw actions
        Transfer,// Transfer transactions between accounts
        Buy,
        Sell
    }

    public enum CurrencyBuyAndSellType
    {
        Cash,
        NonCash
    }

    public static class Expressions
    {
        public static readonly Dictionary<UserRole, string> UserRoleDictionary = new Dictionary<UserRole, string>
    {
        { UserRole.Owner, "صاحب برنامه" },
        { UserRole.User, "کاربر" },
        { UserRole.SuperAdmin, "سوپرادمین" },
        { UserRole.Admin, "ادمین" },
    };


        public static readonly Dictionary<CustomerStatus, string> CustomerStatusDictionary = new Dictionary<CustomerStatus, string>
    {
        { CustomerStatus.Debtor, "بدهکار" },
        { CustomerStatus.Creditor, "بستانکار" },
    };


        public static readonly Dictionary<CommisionType, string> CommisionDictionary = new Dictionary<CommisionType, string>
    {
        { CommisionType.Cash, "نقدی" },
        { CommisionType.NoComission, "بدون کمشن" },
        { CommisionType.FromReciever, "از حساب گیرنده" },
        { CommisionType.FromSender, "از حساب فرستنده" },
    };

        public static readonly Dictionary<TransactionType, string> TransactionTypeDictionary = new Dictionary<TransactionType, string>
    {
        { TransactionType.Normal, "عادی" },
        { TransactionType.Transfer, "انتقال وجه" },
        { TransactionType.Buy, "خرید" },
        { TransactionType.Sell, "فروش" },
    };

        public static readonly Dictionary<string, string> Settings = new Dictionary<string, string>
    {
        { "CashReceiptAndWithdrawal", "رسید و برد نقدی" },
        { "RecieptsRegistration", "ثبت رسیدات" }
    };

        public static readonly Dictionary<DealType, string> DealTypeDictionary = new Dictionary<DealType, string>
    {
        {DealType.Deposit, "رسید" },
        {DealType.Withdraw, "برد" },

    };

        public static readonly Dictionary<Currency, string> CurrencyDictionary = new Dictionary<Currency, string>
    {
        {Currency.Dollar, "دالر" },
        { Currency.Euro, "یورو" },
        { Currency.Afghani, "افغانی" },
        { Currency.Kaldar, "کلدار" },
        { Currency.Toman, "تومان" },
    }; public static readonly Dictionary<eAccountType, string> AccountTypeDictionary = new Dictionary<eAccountType, string>
    {
        {eAccountType.Treasury, "خزانه" },
        {eAccountType.Incremental, "افزایشی" },
        {eAccountType.Decremental, "کاهشی" },
        {eAccountType.Regular, "نورمال" },
        {eAccountType.CurrencyExchange, "تبادله اسعار" },
    };



    }
}
