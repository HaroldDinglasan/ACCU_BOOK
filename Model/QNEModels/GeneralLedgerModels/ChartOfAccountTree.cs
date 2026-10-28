namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class ChartOfAccountTree
{
    //SELF REFERENCE HIERCHY
    //FOR TABLE LISTING
    public Guid NewId { get; set; } = Guid.NewGuid();
    public int ChartSequence
    {
        get; set;
    } // Accounts.ChartSequence
    public int ChartSequencePH
    {
        get; set;
    } // Accounts.ChartSequencePH
    public Guid Id
    {
        get; set;
    }// GLAccount.Id and Account.Id
    public Guid? GLTaxAccountId
    {
        get; set;
    }
    public Guid GLAccountId
    {
        get; set;
    }
    public Guid AcctId
    {
        get; set;
    }// Account.Id
    public Guid AccountId
    {
        get; set;
    }// GLAccount.Id and Account.AccountId
    public string GLAccountCode
    {
        get; set;
    } // GLAccount.GLAccountCode
    public int CurrencyId { get; set; } = 1;
    public int? GSTTypeId { get; set; } = null;
    public string CurrencyCode
    {
        get; set;
    } // dbo.Currencies.CurrencyCode
    public int? SpecialAccountId { get; set; }
    public string SpecialAccountCode
    {
        get; set;
    } // sa.SpecialAccountCode
    public string SpecialAccount
    {
        get; set;
    } // sa.Description AS SpecialAccount
    public string Description
    {
        get; set;
    } // GLAccount.Description
    public string Description2
    {
        get; set;
    }
    public string GLDescription
    {
        get; set;
    } // GLAccount.Description
    public string AcctDescription
    {
        get; set;
    } // GLAccount.Description
    public Guid ParentId
    {
        get; set;
    } // GLAccount.ParentId
    public bool IsActive { get; set; } = true;
    public string GSTGroup
    {
        get; set;
    }
    public bool IsRevaluation { get; set; } = false;
    public string DRCR
    {
        get; set;
    } // GLAccount.DRCR
    public bool IsCreditCard { get; set; } = false;
    public string Notes
    {
        get; set;
    } // GLAccount.Notes
    public bool IsSubAccount { get; set; } = false;
    public string BankAccountNo
    {
        get; set;
    }
    public string GSTMSICCode
    {
        get; set;
    }
    public int OptimisticLockField
    {
        get; set;
    }
    public string SAC
    {
        get; set;
    }
    public int RowIndex
    {
        get; set;
    } // GLAccount.Notes
    public bool HasChildInCoa { get; set; } = false;
    public bool IncludeInCashFlowForecastAdvisor { get; set; } = false;
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
    public string AccountCode
    {
        get; set;
    } // Accounts.AccountCode
    public string Account
    {
        get; set;
    } // Accounts.Account
    public Guid? InputTaxId
    {
        get; set;
    } // dbo.GLAccountTaxCodeLinks.DefaultInputTaxCodeId
    public Guid? OutputTaxId
    {
        get; set;
    } // dbo.GLAccountTaxCodeLinks.DefaultOutputTaxCodeId
    public string InputTax
    {
        get; set;
    } // dbo.GLAccountTaxCodeLinks.DefaultInputTaxCodeId
    public string OutputTax
    {
        get; set;
    } // dbo.GLAccountTaxCodeLinks.DefaultOutputTaxCodeId
}
