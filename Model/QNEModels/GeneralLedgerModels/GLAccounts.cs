using System.ComponentModel.DataAnnotations;

namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class GLAccounts
{
    [Key]

    public Guid Id { get; set; } = Guid.NewGuid();

    //NULLABLE
    public Guid? ParentId
    {
        get; set;
    }


    //Sample Data Crud GLAccountCode
    [Required(ErrorMessage = "'Account Code' must not be empty.")]
    public string GLAccountCode
    {
        get; set;
    }

    //NULLABLE
    [Required(ErrorMessage = "'Account Type' must not be empty.")]
    public Guid? AccountId
    {
        get; set;
    }

    //NULLABLE
    public int? SpecialAccountId
    {
        get; set;
    }


    //NULLABLE
    //Default value 1 as PHP currency
    public int? CurrencyId { get; set; } = 1;

    //NULLABLE
    public int? GSTTypeId { get; set; } = null;


    [Required(ErrorMessage = "'Account Name' must not be empty.")]
    public string Description
    {
        get; set;
    }

    //NULLABLE
    //Default Selected Value DR as DEBIT
    public string DRCR { get; set; } = "DR";
    //NULLABLE
    public bool IsCreditCard { get; set; } = false;


    public bool IsActive { get; set; } = true;

    //NULLABLE
    public string GSTGroup
    {
        get; set;
    }
    public bool IsRevaluation { get; set; } = false;
    public string Notes
    {
        get; set;
    }
    public bool IsSubAccount { get; set; } = false;
    public string BankAccountNo
    {
        get; set;
    }
    public string GSTMSICCode
    {
        get; set;
    }
    public int OptimisticLockField { get; set; } = 0;
    public string SAC
    {
        get; set;
    }
    //END THIS NULLABLE COLUMN

    //AUTO GENARATED INDEX
    public int RowIndex
    {
        get; set;
    }

    public bool HasChildInCoa { get; set; } = false;

    public bool IncludeInCashFlowForecastAdvisor { get; set; } = false;

    //NULLABLE
    public string SSTTariffCode
    {
        get; set;
    }
    public int? TariffCodeId
    {
        get; set;
    }
    public Guid? ATCCodeId
    {
        get; set;
    }
    public string Description2
    {
        get; set;
    }
    //END THIS NULLABLE COLUMN











    public string AccountCodeDescription
    {
        get
        {
            return $"{GLAccountCode}: {Description}";
        }
    }



}
