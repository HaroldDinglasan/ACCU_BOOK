using Accounting_V4.Services.QNEServices.AllBasicServices;
using Accounting_V4.Services.SystemServices.TimeZoneServices;

namespace Accounting_V4.Model.QNEModels.SalesModels;

public class SalesInvoicesHeader
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SalesInvoiceCode
    {
        get; set;
    }
    public DateTime SalesInvoiceDate
    {
        get; set;
    } = TimeService.ServerTime;
    public Guid? DebtorId
    {
        get; set;
    }
    public string DebtorName
    {
        get; set;
    }
    public string Attention
    {
        get; set;
    }
    public string Phone
    {
        get; set;
    }
    public string Fax
    {
        get; set;
    }
    public string Address1
    {
        get; set;
    }
    public string Address2
    {
        get; set;
    }
    public string Address3
    {
        get; set;
    }
    public string Address4
    {
        get; set;
    }
    public string ReferenceNo
    {
        get; set;
    }
    public string OurDONO
    {
        get; set;
    }
    public int? TermId
    {
        get; set;
    } // Checked
    public DateTime DueDate
    {
        get; set;
    }
    public int? SalesPersonId
    {
        get; set;
    } // Checked
    public string DeliveryTerm
    {
        get; set;
    }
    public string Title
    {
        get; set;
    }
    public string Title2
    {
        get; set;
    }
    public string Ref1
    {
        get; set;
    }
    public string Ref2
    {
        get; set;
    }
    public string Ref3
    {
        get; set;
    }
    public string Ref4
    {
        get; set;
    }
    public string Ref5
    {
        get; set;
    }
    public string Remark1
    {
        get; set;
    }
    public string Remark2
    {
        get; set;
    }
    public string Remark3
    {
        get; set;
    }
    public string Remark4
    {
        get; set;
    }
    public string Remark5
    {
        get; set;
    }
    public int? ProjectId
    {
        get; set;
    } // Checked
    public int? DepartmentId
    {
        get; set;
    } // Checked
    public Guid? StockLocationId
    {
        get; set;
    } // Checked
    public int CurrencyId
    {
        get; set;
    }
    public decimal CurrencyRate
    {
        get; set;
    }
    public decimal TotalAmount
    {
        get; set;
    }
    public decimal NetTotalAmountLocal
    {
        get; set;
    }
    public bool IsCancelled
    {
        get; set;
    }
    public string DOBranchCode
    {
        get; set;
    }
    public string DOBranchName
    {
        get; set;
    }
    public string DOContact
    {
        get; set;
    }
    public string DOPhone
    {
        get; set;
    }
    public string DOFax
    {
        get; set;
    }
    public string DOAddress1
    {
        get; set;
    }
    public string DOAddress2
    {
        get; set;
    }
    public string DOAddress3
    {
        get; set;
    }
    public string DOAddress4
    {
        get; set;
    }
    public int? CostCentreId
    {
        get; set;
    } // Checked
    public string Discount
    {
        get; set;
    }
    public decimal? DiscountAmount
    {
        get; set;
    } // Checked
    public decimal NetTotalAmount
    {
        get; set;
    }
    public decimal DiscountAmountLocal
    {
        get; set;
    }
    public decimal TotalAmountLocal
    {
        get; set;
    }
    public decimal? TaxTotalAmount
    {
        get; set;
    } // Checked
    public decimal? TaxTotalAmountLocal
    {
        get; set;
    } // Checked
    public decimal? SubtotalAmount
    {
        get; set;
    } // Checked
    public decimal? SubtotalAmountLocal
    {
        get; set;
    } // Checked
    public string ExportCountry
    {
        get; set;
    }
    public string Notes
    {
        get; set;
    }
    public bool IsReliefTax
    {
        get; set;
    } = false;
    public bool IsTaxInclusive
    {
        get; set;
    } // Checked
    public DateTime? TaxDate
    {
        get; set;
    } = TimeService.ServerTime; // Checked
    public decimal? TaxExclusiveTotalAmount
    {
        get; set;
    } // Checked
    public decimal? TaxExclusiveTotalAmountLocal
    {
        get; set;
    } // Checked
    public bool? IsTaxInclusiveOnly
    {
        get; set;
    } // Checked
    public Guid? RoundingAdjustmentAccount
    {
        get; set;
    } // Checked
    public decimal? RoundingAdjustment
    {
        get; set;
    } = 0.00M;
    public decimal? RoundingAdjustmentLocal
    {
        get; set;
    } = 0.00M;
    public bool? IsRounding
    {
        get; set;
    } = false;
    public int OptimisticLockField
    {
        get; set;
    } = 0;
    public decimal? WTaxTotalAmount
    {
        get; set;
    } // Checked
    public decimal? WTaxTotalAmountLocal
    {
        get; set;
    } // Checked
    public string DORegistationNo
    {
        get; set;
    }
    public string DOGSTRegNo
    {
        get; set;
    }
    public string DOPhone2
    {
        get; set;
    }
    public string DOEmail
    {
        get; set;
    }
    public string? DORemark
    {
        get; set;
    }
    public int? DeliveryArea
    {
        get; set;
    } // Checked
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
    public string TransferFromInfo
    {
        get; set;
    }
    public bool IsApproved
    {
        get; set;
    } = true;
    public bool IsDeferredVAT
    {
        get; set;
    } = false;
    public bool? IsDifferentPOS
    {
        get; set;
    } = false;
    public int? PlaceOfSupplyId
    {
        get; set;
    } // Checked
    public bool? ReverseCharge
    {
        get; set;
    } = false;
    public bool? IsPaidUnderPA
    {
        get; set;
    } = false;
    public string EGSTIN
    {
        get; set;
    }
    public bool? IsAmendedInvoice
    {
        get; set;
    } = false;
    public decimal ReverseRate
    {
        get; set;
    } = 0.00000000M;
    public string SIRef1
    {
        get; set;
    }
    public string SIRef2
    {
        get; set;
    }
    public decimal? TotalServiceAmount
    {
        get; set;
    } = 0.00M;
    public decimal? TotalServiceAmountLocal
    {
        get; set;
    } = 0.00M;
    public int? FixedCommissionInfoId
    {
        get; set;
    } // Checked
    public bool? IsNonTaxInvoice
    {
        get; set;
    } = false;
    public DateTime? PostingDate
    {
        get; set;
    } // Checked
    public decimal? TotalPaid
    {
        get; set;
    } = 0.00M;
    public decimal? AdjustmentAmount
    {
        get; set;
    } = 0.00M;
    public decimal? TotalMatchedAmount
    {
        get; set;
    } = 0.00M;
    public decimal? GlobalProgressInvoicingRate
    {
        get; set;
    } = 0.00M;
    public int? PackingListId
    {
        get; set;
    } // Checked
    public string BillerCode
    {
        get; set;
    }
    public int JomPayRefType1
    {
        get; set;
    } = 0;
    public string JomPayRef1
    {
        get; set;
    } = "Documnet #";
    public int JomPayRefType2
    {
        get; set;
    } = 0;
    public string JomPayRef2
    {
        get; set;
    } = "Documnet #";
    public bool UseCreditCard
    {
        get; set;
    } = false;
    public bool? IsSSTBadDebt
    {
        get; set;
    } = false; // Checked
    public decimal? WVatTotalAmount
    {
        get; set;
    } = 0.00M;// Checked
    public decimal? WVatTotalAmountLocal
    {
        get; set;
    } = 0.00M;// Checked
    public bool? IsESales
    {
        get; set;
    } = false; // Checked
    public bool? IsSelfBearWTax
    {
        get; set;
    } = false; // Checked
    public bool IsDetailPosting
    {
        get; set;
    } = false; // Checked
    public string ZipCode
    {
        get; set;
    }
    public string City
    {
        get; set;
    }
    public int? StateId
    {
        get; set;
    } // Checked
    public int? CountryId
    {
        get; set;
    } // Checked
    public int TinType
    {
        get; set;
    } = 0;
    public string? CustomerTIN
    {
        get; set;
    }
    public int? DimensionId
    {
        get; set;
    } // Checked
    public decimal TotalAmountTaxExempted
    {
        get; set;
    } = 0.00M; // Checked
    public decimal TotalAmountTaxExemptedLocal
    {
        get; set;
    } = 0.00M; // Checked
    public bool? IsEinvoiceReject
    {
        get; set;
    } = false; // Checked   
    public Guid? CorrectionCode
    {
        get; set;
    } // Checked
    public string EISId
    {
        get; set;
    } = EISIdService.Generate();
    public Guid? PostingAccountGroupId
    {
        get; set;
    } // Checked
    public string SiteGiantOrderId
    {
        get; set;
    }
    public string DebtorRegistrationNo
    {
        get; set;
    }
    public int? DebtorTINType
    {
        get; set;
    }
    public string DebtorTIN
    {
        get; set;
    }
    public string CustomsReferenceNo
    {
        get; set;
    }
    public string FTAInformation
    {
        get; set;
    }
    public string CustomsFormNo2
    {
        get; set;
    }
    public string Incoterms
    {
        get; set;
    }
    public int? ShippingCountryId
    {
        get; set;
    } // Checked
    public string OtherCharges
    {
        get; set;
    }
    public string CertifiedExporterNo
    {
        get; set;
    }
    public int? EInvoiceStatus
    {
        get; set;
    } // Checked
    public Guid? SubmittedById
    {
        get; set;
    } // Checked
    public DateTime? SubmissionDate
    {
        get; set;
    } // Checked
    public DateTime? ValidatedOn
    {
        get; set;
    } // Checked
    public DateTime? BillingFrom
    {
        get; set;
    } // Checked
    public DateTime? BillingTo
    {
        get; set;
    } // Checked
    public Guid? CancelledById
    {
        get; set;
    } // Checked
    public DateTime? CancellationDate
    {
        get; set;
    } // Checked
    public string CancellationReason
    {
        get; set;
    }
    public string UniqueIdentifiedNo
    {
        get; set;
    }
    public string TaxExemptionNo
    {
        get; set;
    }
    public string TaxExemptionDetails
    {
        get; set;
    }
    public Guid? ConsolidatedEInvoiceId
    {
        get; set;
    } // Checked
    public int? CustomerCountryId
    {
        get; set;
    } // Checked
    public string CustomerCity
    {
        get; set;
    }
    public int? CustomerCountryStateId
    {
        get; set;
    } // Checked
    public string CustomerZipCode
    {
        get; set;
    }
    public string CustomerSalesTaxRegNo
    {
        get; set;
    }
    public string CustomerServiceTaxRegNo
    {
        get; set;
    }


    public Guid? glaccountTAXSALESACCOUNT
    {
        get; set;
    }
    public Guid? glaccountOUPUTVAT
    {
        get; set;
    }



}
