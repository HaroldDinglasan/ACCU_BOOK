using System.ComponentModel.DataAnnotations;

namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class SubAccounts
{
    [Key]
    public Guid Id
    {
        get; set;
    }
    public Guid ParentId
    {
        get; set;
    }
    public Guid AccountId
    {
        get; set;
    }
    public string GLAccountCode
    {
        get; set;
    }
    public int SpecialAccountId
    {
        get; set;
    }
    //AccountId
    //SpecialAccountId
    //CurrencyId
    //GSTTypeId
    public string Description
    {
        get; set;
    }

    public string AccountCodeDescription
    {
        get
        {
            return $"{GLAccountCode}: {Description}";
        }
    }
    public string DRCR
    {
        get; set;
    }
    //IsCreditCard
    //IsActive
    //GSTGroup
    //IsRevaluation
    //Notes
    //IsSubAccount
    //BankAccountNo
    //GSTMSICCode
    //OptimisticLockField
    //SAC
    //RowIndex
    //HasChildInCoa
    //IncludeInCashFlowForecastAdvisor
    //SSTTariffCode
    //TariffCodeId
    //ATCCodeId
    //Description2
}
