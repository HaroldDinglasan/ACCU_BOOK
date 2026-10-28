using System.ComponentModel.DataAnnotations;

namespace Accounting_V4.Model.QNEModels.CustomerModels;

public class Debtors
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CompanyCode
    {
        get; set;
    }
    public string CompanyName
    {
        get; set;
    }
    public string CompanyName2
    {
        get; set;
    }

    public Guid GLAccountId
    {
        get; set;
    }
    public Guid? ControlAccountId
    {
        get; set;
    }
    public string RegistrationNo
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
    public string ContactPerson
    {
        get; set;
    }
    public string PhoneNo1
    {
        get; set;
    }
    public string PhoneNo2
    {
        get; set;
    }
    public string FaxNo1
    {
        get; set;
    }
    public string FaxNo2
    {
        get; set;
    }
    public string Email
    {
        get; set;
    }
    public string Homepage
    {
        get; set;
    }
    public string BusinessNature
    {
        get; set;
    }
    public byte[] Picture
    {
        get; set;
    } // Assuming Picture is a binary image
    public int? CategoryId
    {
        get; set;
    }
    public int? AreaId
    {
        get; set;
    }
    public int? SalesPersonId
    {
        get; set;
    }
    public int CurrencyId { get; set; } = 1;
    public int? TermId
    {
        get; set;
    }
    public decimal? CreditLimit
    {
        get; set;
    }
    public bool? IsExceedCreditAllowed
    {
        get; set;
    }
    public bool? IsTaxExempted
    {
        get; set;
    }
    public int BillingCycleId { get; set; } = 2;
    public int StatementCycleId { get; set; } = 2;
    public string BillingContactPerson
    {
        get; set;
    }
    public string BillingContactPhone
    {
        get; set;
    }
    public string BillingContactEmail
    {
        get; set;
    }
    public string AccountContactPerson
    {
        get; set;
    }
    public string AccountContactPhone
    {
        get; set;
    }
    public string AccountContactEmail
    {
        get; set;
    }
    public string ManagementContactPerson
    {
        get; set;
    }
    public string ManagementContactPhone
    {
        get; set;
    }
    public string ManagementContactEmail
    {
        get; set;
    }
    public string GuarantorName
    {
        get; set;
    }
    public string GuarantorIdNo
    {
        get; set;
    }
    public string GuarantorGender
    {
        get; set;
    }
    public string GuarantorRace
    {
        get; set;
    }
    public string GuarantorCitizenship
    {
        get; set;
    }
    public decimal? CurrentBalance
    {
        get; set;
    }
    public int? StatementTypeId
    {
        get; set;
    }
    public int? GSTTypeId
    {
        get; set;
    }
    public string GSTExemptionNo
    {
        get; set;
    }
    public DateTime? GSTExpiryDate
    {
        get; set;
    }
    public float? AdditionalCreditLimit
    {
        get; set;
    }
    public DateTime? AdditionalCreditLimitExpiryDate
    {
        get; set;
    }
    public string SalesTaxExemptionNo
    {
        get; set;
    }
    public DateTime? SalesTaxExemptionExpiryDate
    {
        get; set;
    }
    public int? UserPriceGroupId
    {
        get; set;
    }
    public int? PriceGroupId
    {
        get; set;
    }
    public string UserPriceTag
    {
        get; set;
    }
    public string PriceTag
    {
        get; set;
    }
    public string SalesTypeId
    {
        get; set;
    }
    public DateTime StartDate { get; set; } = DateTime.Now.Date;
    public string SourceOfLead
    {
        get; set;
    }
    public string MemberName
    {
        get; set;
    }
    public string MemberOccupation
    {
        get; set;
    }
    public DateTime? MemberSince
    {
        get; set;
    }
    public string MemberIdNo
    {
        get; set;
    }
    public DateTime? MemberBirthday
    {
        get; set;
    }
    public string MemberEmail
    {
        get; set;
    }
    public string MemberMobileNo
    {
        get; set;
    }
    public string MemberRace
    {
        get; set;
    }
    public string MemberReligion
    {
        get; set;
    }
    public DateTime? MemberValidThru
    {
        get; set;
    }
    public string SMSGroupCode
    {
        get; set;
    }
    public string SMSNo
    {
        get; set;
    }
    public string Status
    {
        get; set;
    }
    public string Agent
    {
        get; set;
    }
    public string GSTRegNo
    {
        get; set;
    }
    public Guid? DefaultTaxCodeId
    {
        get; set;
    }
    public string Notes
    {
        get; set;
    }
    public int OptimisticLockField
    {
        get; set;
    }
    public Guid? BIRClassification
    {
        get; set;
    }
    public string GSTRegTag
    {
        get; set;
    }
    public Guid? DefaultWTaxCodeId
    {
        get; set;
    }
    public int? StateId
    {
        get; set;
    }
    public bool IsGroupCompany { get; set; } = false;
    public Guid? GroupCompanyId
    {
        get; set;
    }
    public string BranchCode
    {
        get; set;
    }
    public Guid? PostingAccountGroupId
    {
        get; set;
    }
    public int RowIndex
    {
        get; set;
    }
    public bool UseLocalCreditControl { get; set; } = false;
    public bool ExcludeSOCreditControl { get; set; } = false;
    public bool ExcludeDOCreditControl { get; set; } = false;
    public string BusinessStyle
    {
        get; set;
    }
    public string SalesTaxRegNo
    {
        get; set;
    }
    public string ServiceTaxRegNo
    {
        get; set;
    }
    public DateTime? SalesTaxExemptionExpDate
    {
        get; set;
    }
    public bool IsDeferredVAT { get; set; } = false;
    public string ZipCode
    {
        get; set;
    }
    public int? GroupId
    {
        get; set;
    }
    public Guid? DefaultWVatCodeId
    {
        get; set;
    }
    public int? PointCategoryId
    {
        get; set;
    }
    public string PointStatus
    {
        get; set;
    }
    public DateTime? PointExpiryDate
    {
        get; set;
    }
}
