namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class ReceiptDetails
{
    public Guid Id
    {
        get; set;
    } = Guid.NewGuid();
    public Guid ReceiptId
    {
        get; set;
    }
    public int Pos
    {
        get; set;
    }

    public Guid GLAccountId
    {
        get; set;
    }
    public string? Description
    {
        get; set;
    }
    public int? ProjectId
    {
        get; set;
    }
    public int? DepartmentId
    {
        get; set;
    }
    public string? ReferenceNo
    {
        get; set;
    }

    public decimal Amount
    {
        get; set;
    } = 0.00M;
    public bool? IsProcessed
    {
        get; set;
    }

    public int? WTaxTypeId
    {
        get; set;
    }
    public decimal WTaxAmount
    {
        get; set;
    } = 0.00000000M;

    public int? GSTTypeId
    {
        get; set;
    }
    public decimal? GSTAmount
    {
        get; set;
    }
    public decimal? TaxableSales
    {
        get; set;
    }

    public int? CostCentreId
    {
        get; set;
    }

    public decimal AmountLocal
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

    public int TaxClass
    {
        get; set;
    } = 0;
    public string? TaxClassValue
    {
        get; set;
    }

    public decimal? TaxAmount
    {
        get; set;
    } = 0.00000000M;
    public decimal? TaxAmountLocal
    {
        get; set;
    } = 0.00000000M;

    public decimal NetAmount
    {
        get; set;
    } = 0.00M;
    public decimal NetAmountLocal
    {
        get; set;
    } = 0.00M;

    public decimal SubAmount
    {
        get; set;
    } = 0.00M;
    public decimal SubAmountLocal
    {
        get; set;
    } = 0.00M;

    public string? TaxClassValueRef
    {
        get; set;
    }

    public decimal? TaxExclusiveAmount
    {
        get; set;
    } = 0.00M;
    public decimal? TaxExclusiveAmountLocal
    {
        get; set;
    } = 0.00M;

    public int OptimisticLockField
    {
        get; set;
    } = 0;

    public Guid? WTaxCode
    {
        get; set;
    }
    public decimal? WTaxRate
    {
        get; set;
    } = 0.00000000M;
    public decimal WTaxAmountLocal
    {
        get; set;
    } = 0.00000000M;

    public Guid? PaymentReceiptInfoId
    {
        get; set;
    }
    public Guid? GLBIRClsInfo
    {
        get; set;
    }

    public int RowIndex
    {
        get; set;
    }

    public int? TariffCodeId
    {
        get; set;
    }

    public DateTime? ServicePostingDate
    {
        get; set;
    }

    public Guid? ATCCodeId
    {
        get; set;
    }

    public Guid? WVatCode
    {
        get; set;
    }
    public decimal? WVatRate
    {
        get; set;
    } = 0.00M;
    public decimal? WVatAmount
    {
        get; set;
    } = 0.00M;
    public decimal? WVatAmountLocal
    {
        get; set;
    } = 0.00M;

    public int? DimensionId
    {
        get; set;
    }
}
