using System.ComponentModel.DataAnnotations;

namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class GeneralLedgerAccount
{
    //POPULATE ALL TABLE'S COLUMN
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ParentId
    {
        get; set;
    }
    public string AccountNumber
    {
        get; set;
    }
    public string AccountName
    {
        get; set;
    }
    public string AccountType
    {
        get; set;
    }
    public string SpecialAccount
    {
        get; set;
    }
    public string BankAccountNumber
    {
        get; set;
    }
    public int CurrencyId
    {
        get; set;
    }
    public string Currency
    {
        get; set;
    }
    public string Description2
    {
        get; set;
    }
    public string DRCR
    {
        get; set;
    }
    public bool IsActive { get; set; } = true;
    public string Notes
    {
        get; set;
    }

    public bool IsSubAccount
    {
        get; set;
    }
    public string SACode
    {
        get; set;
    }

    public bool IsCreditCard { get; set; } = false;
    public string GSTGroup
    {
        get; set;
    }
    public bool IsRevaluation { get; set; } = false;
    public string AccountCode
    {
        get; set;
    }
    public int SpecialAccountId
    {
        get; set;
    }
    public Guid AccountId
    {
        get; set;
    }
    public int RowIndex
    {
        get; set;
    }
    public int GSTTypeId
    {
        get; set;
    }

    public string DefaultInputTaxCode
    {
        get; set;
    }
    public string DefaultOutputTaxCode
    {
        get; set;
    }


    public IEnumerable<GeneralLedgerAccount> SubAccounts
    {
        get; set;
    }
}
