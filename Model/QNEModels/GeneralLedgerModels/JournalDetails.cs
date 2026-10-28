namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class JournalDetails
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JournalId
    {
        get; set;
    }
    public int Pos
    {
        get; set;
    }
    public Guid? GLAccountId
    {
        get; set;
    }
    public string? Description
    {
        get; set;
    }
    public string? ReferenceNo
    {
        get; set;
    }
    public decimal Debit
    {
        get; set;
    } = 0.00M;
    public decimal Credit
    {
        get; set;
    } = 0.00M;
    public int? ProjectId
    {
        get; set;
    }
    public int? DepartmentId
    {
        get; set;
    }
    public int? GSTType
    {
        get; set;
    }
    public string? GSTGroup
    {
        get; set;
    }
    public decimal? GSTAmount
    {
        get; set;
    }
    public decimal? GSTTaxAmount
    {
        get; set;
    }
    public decimal? TaxablePurchases
    {
        get; set;
    }
    public decimal? TaxableSales
    {
        get; set;
    }
    public decimal? TaxablePurchase
    {
        get; set;
    }
    public int? CostCentreId
    {
        get; set;
    }
    public decimal DebitLocal
    {
        get; set;
    } = 0.00M;
    public decimal CreditLocal
    {
        get; set;
    } = 0.00M;
    public string? Note
    {
        get; set;
    }
    public string? ItemTypeCode
    {
        get; set;
    }
    public Guid? TaxCodeId
    {
        get; set;
    }
    public bool IsTaxInclusive
    {
        get; set;
    } = false;
    public decimal? TaxRate
    {
        get; set;
    } = 0.0000M;
    public int? TaxClass
    {
        get; set;
    } = 0;
    public string? TaxClassValue
    {
        get; set;
    }
    public decimal TaxDebit
    {
        get; set;
    } = 0.00000000M;
    public decimal TaxDebitLocal
    {
        get; set;
    } = 0.00000000M;
    public decimal TaxCredit
    {
        get; set;
    } = 0.00000000M;
    public decimal TaxCreditLocal
    {
        get; set;
    } = 0.00000000M;
    public decimal NetDebit
    {
        get; set;
    } = 0.00M;
    public decimal NetCredit
    {
        get; set;
    } = 0.00M;
    public decimal? NetDebitLocal
    {
        get; set;
    } = 0.00M;
    public decimal? NetCreditLocal
    {
        get; set;
    } = 0.00M;
    public bool? IsOutputTax
    {
        get; set;
    }
    public Guid? BDJournalDetailId
    {
        get; set;
    }
    public Guid? BDInvoiceDetailId
    {
        get; set;
    }
    public Guid? BDARDNDetailId
    {
        get; set;
    }
    public Guid? BDBillDetailId
    {
        get; set;
    }
    public Guid? BDAPDNDetailId
    {
        get; set;
    }
    public Guid? RecoveredJournalDetailId
    {
        get; set;
    }
    public string? TaxClassValueRef
    {
        get; set;
    }
    public decimal TaxExclusiveDebit
    {
        get; set;
    } = 0.00M;
    public decimal TaxExclusiveCredit
    {
        get; set;
    } = 0.00M;
    public decimal TaxExclusiveDebitLocal
    {
        get; set;
    } = 0.00M;
    public decimal TaxExclusiveCreditLocal
    {
        get; set;
    } = 0.00M;
    public int OptimisticLockField
    {
        get; set;
    } = 0;
    public Guid? BDPastInvoiceId
    {
        get; set;
    }
    public Guid? BDPastARDNId
    {
        get; set;
    }
    public Guid? BDPastBillId
    {
        get; set;
    }
    public Guid? BDPastAPDNId
    {
        get; set;
    }
    public Guid? BDBillId
    {
        get; set;
    }
    public Guid? GLBIRClsInfo
    {
        get; set;
    }
    public Guid? PaymentReceiptInfoId
    {
        get; set;
    }
    public decimal? SgstDebit
    {
        get; set;
    } = 0.00M;
    public decimal? CgstDebit
    {
        get; set;
    } = 0.00M;
    public decimal? IgstDebit
    {
        get; set;
    } = 0.00M;
    public decimal? SgstDebitLocal
    {
        get; set;
    } = 0.00M;
    public decimal? CgstDebitLocal
    {
        get; set;
    } = 0.00M;
    public decimal? IgstDebitLocal
    {
        get; set;
    } = 0.00M;
    public decimal? SgstCredit
    {
        get; set;
    } = 0.00M;
    public decimal? CgstCredit
    {
        get; set;
    } = 0.00M;
    public decimal? IgstCredit
    {
        get; set;
    } = 0.00M;
    public decimal? SgstCreditLocal
    {
        get; set;
    } = 0.00M;
    public decimal? CgstCreditLocal
    {
        get; set;
    } = 0.00M;
    public decimal? IgstCreditLocal
    {
        get; set;
    } = 0.00M;
    public int RowIndex
    {
        get; set;
    }
    public int? TariffCodeId
    {
        get; set;
    }
    public Guid? RecoveredPastInvoiceId
    {
        get; set;
    }
    public Guid? BDJournalId
    {
        get; set;
    }
    public Guid? BDInvoiceId
    {
        get; set;
    }
    public Guid? BDARDNId
    {
        get; set;
    }
    public Guid? BDAPDNId
    {
        get; set;
    }
    public bool IsCreditable
    {
        get; set;
    } = false;

    public decimal? WTaxRate
    {
        get; set;
    }= 0.0000M;
    public Guid? WTaxCode
    {
        get; set;
    }
    public decimal WTaxDebit
    {
        get; set;
    } = 0.00000000M;
    public decimal WTaxCredit
    {
        get; set;
    } = 0.00000000M;
    public bool IsDeferredVAT
    {
        get; set;
    } = false;
    public Guid? ServiceTaxCode
    {
        get; set;
    }
    public DateTime? ServicePostingDate
    {
        get; set;
    }
    public decimal? ServiceTaxAmount
    {
        get; set;
    } = 0.00M;
    public decimal? LocalOneCentsDiff
    {
        get; set;
    }
    public int? DimensionId
    {
        get; set;
    }
    public string? UniqueIdentifiedNo
    {
        get; set;
    }
}
