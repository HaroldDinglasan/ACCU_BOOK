namespace Accounting_V4.Model.QNEModels.InvoiceModels;

public class InvoiceDetails
{
    public Guid Id
    {
        get; set;
    } = Guid.NewGuid();
    public Guid InvoiceId
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
    public int? ProjectId
    {
        get; set;
    }
    public int? DepartmentId
    {
        get; set;
    }
    public int? CostCentreId
    {
        get; set;
    }
    public decimal Amount
    {
        get; set;
    } = 0.00M;
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
    }
    public decimal? TaxRate
    {
        get; set;
    }
    public int? TaxClass
    {
        get; set;
    }
    public string? TaxClassValue
    {
        get; set;
    }
    public decimal TaxAmount
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
    public decimal? TaxExclusiveAmount
    {
        get; set;
    } = 0.00M;
    public decimal? TaxExclusiveAmountLocal
    {
        get; set;
    } = 0.00M;
    public string? TaxClassValueRef
    {
        get; set;
    }
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
    public decimal? WTaxAmount
    {
        get; set;
    } = 0.00000000M;
    public decimal? WTaxAmountLocal
    {
        get; set;
    } = 0.00000000M;
    public decimal? SgstAmount
    {
        get; set;
    } = 0.00000000M;
    public decimal CgstAmount
    {
        get; set;
    } = 0.00000000M;
    public decimal? IgstAmount
    {
        get; set;
    } = 0.00000000M;
    public decimal SgstAmountLocal
    {
        get; set;
    } = 0.00000000M;
    public decimal? CgstAmountLocal
    {
        get; set;
    } = 0.00000000M;
    public decimal? IgstAmountLocal
    {
        get; set;
    } = 0.00000000M;
    public int RowIndex
    {
        get; set;
    }
    public int? TariffCodeId
    {
        get; set;
    }
    public decimal? LocalOneCentsDiff
    {
        get; set;
    }
    public decimal? ServiceAmount
    {
        get; set;
    }
    public decimal? ServiceAmountLocal
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
    } = 0.00000000M;
    public decimal? WVatAmountLocal
    {
        get; set;
    } = 0.00000000M;
    public Guid? ATCCodeId
    {
        get; set;
    }
    public int? DimensionId
    {
        get; set;
    }
}
