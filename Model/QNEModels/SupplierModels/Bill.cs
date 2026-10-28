using Accounting_V4.Services.SystemServices.TimeZoneServices;

namespace Accounting_V4.Model.QNEModels.SupplierModels;

public class Bill
{
    public Guid Id
    {
        get; set;
    } = Guid.NewGuid();

    public string BillCode { get; set; } 

    public DateTime BillDate
    {
        get; set;
    } = TimeService.ServerTime;

    public string? BillFrom
    {
        get; set;
    }

    public Guid? CreditorId
    {
        get; set;
    }



    public string? ReferenceNo
    {
        get; set;
    }

    public int? TermId
    {
        get; set;
    }

    public DateTime DueDate
    {
        get; set;
    }

    public int? PurchaserId
    {
        get; set;
    }

    public int? ProjectId
    {
        get; set;
    }

    public int CurrencyId
    {
        get; set;
    } = 1;

    public decimal? CurrencyRate
    {
        get; set;
    } = 1.00000000M;

    public string? Description
    {
        get; set;
    }

    public string? Description2
    {
        get; set;
    }

    public decimal TotalAmount
    {
        get; set;
    } = 0.00M;

    public bool IsCancelled
    {
        get; set;
    } = false;

    public bool? IsDetailForGLPosting
    {
        get; set;
    }

    public bool IsReceived
    {
        get; set;
    } = false;

    public DateTime PostDate
    {
        get; set;
    } =  TimeService.ServerTime;

    public decimal TotalAmountLocal
    {
        get; set;
    } = 0.00M;

    public int? GSTTypeId
    {
        get; set;
    }

    public decimal? GSTTaxRate
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

    public decimal? GSTAmountLocal
    {
        get; set;
    }

    public decimal? GSTTaxAmountLocal
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

    public string? FromDocType
    {
        get; set;
    }

    public Guid FromDocId
    {
        get; set;
    }

    public decimal TaxTotalAmount
    {
        get; set;
    } = 0.00M;

    public decimal TaxTotalAmountLocal
    {
        get; set;
    } = 0.00M;

    public decimal NetTotalAmount
    {
        get; set;
    } = 0.00M;

    public decimal NetTotalAmountLocal
    {
        get; set;
    } = 0.00M;

    public decimal SubtotalAmount
    {
        get; set;
    } = 0.00M;

    public decimal SubtotalAmountLocal
    {
        get; set;
    } = 0.00M;

    public string? ImportDeclarationNo
    {
        get; set;
    }

    public string? Notes
    {
        get; set;
    }

    public string? ItemDescriptions
    {
        get; set;
    }

    public bool IsTaxInclusive
    {
        get; set;
    } = false;

    public string? CreditorInvNo
    {
        get; set;
    }

    public int? ObjectType
    {
        get; set;
    }

    public decimal TaxExclusiveTotalAmount
    {
        get; set;
    } = 0.00M;

    public decimal TaxExclusiveTotalAmountLocal
    {
        get; set;
    } = 0.00M;

    public DateTime TaxDate
    {
        get; set;
    } = TimeService.ServerTime;

    public bool? IsTaxInclusiveOnly
    {
        get; set;
    }

    public Guid? RoundingAdjustmentAccount
    {
        get; set;
    }

    public decimal RoundingAdjustment
    {
        get; set;
    } = 0.00M;

    public decimal RoundingAdjustmentLocal
    {
        get; set;
    } = 0.00M;

    public bool IsRounding
    {
        get; set;
    } = false;

    public bool IsK1GoodsRemoved
    {
        get; set;
    } = false;

    public Guid? K1GoodsRemovedId
    {
        get; set;
    }

    public int OptimisticLockField
    {
        get; set;
    } = 0;

    public decimal WTaxTotalAmount
    {
        get; set;
    } = 0.00M;

    public decimal WTaxTotalAmountLocal
    {
        get; set;
    } = 0.00M;

    public decimal SgstTotalAmount
    {
        get; set;
    } = 0.00M;

    public decimal SgstTotalAmountLocal
    {
        get; set;
    } = 0.00M;

    public decimal CgstTotalAmount
    {
        get; set;
    } = 0.00M;

    public decimal CgstTotalAmountLocal
    {
        get; set;
    } = 0.00M;

    public decimal IgstTotalAmount
    {
        get; set;
    } = 0.00M;

    public decimal IgstTotalAmountLocal
    {
        get; set;
    } = 0.00M;

    public bool IsImportedServicePosted
    {
        get; set;
    } = false;

    public bool IsDeferredVAT
    {
        get; set;
    } = false;

    public Guid? DocTaxCodeId
    {
        get; set;
    }

    public decimal? DocTaxRate
    {
        get; set;
    }

    public decimal ReverseRate
    {
        get; set;
    } = 0.00000000M;

    public bool IsNonTaxInvoice
    {
        get; set;
    } = false;

    public DateTime? ForecastDueDate
    {
        get; set;
    }

    public decimal TotalPaid
    {
        get; set;
    } = 0.00M;

    public bool IsReliefImported
    {
        get; set;
    } = false;

    public bool IsSelfBearWTax
    {
        get; set;
    } = false;

    public bool IsDetailPosting
    {
        get; set;
    } = false;

    public bool? ExcludeFromKnockoff
    {
        get; set;
    }

    public bool IsEinvoiceReject
    {
        get; set;
    } = false;

    public int? DimensionId
    {
        get; set;
    }

    public decimal TotalServiceAmount
    {
        get; set;
    } = 0.00M;

    public decimal TotalServiceAmountLocal
    {
        get; set;
    } = 0.00M;

    public string? UniqueIdentifiedNo
    {
        get; set;
    }

    public string? ValidatedDate
    {
        get; set;
    }


    /// <summary>
    /// ADDITIONAL ID
    /// </summary>
    public Guid? GLAccountId
    {
        get; set;
    }

}
