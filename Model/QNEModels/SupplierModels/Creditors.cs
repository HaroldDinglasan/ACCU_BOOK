using System.ComponentModel.DataAnnotations;

namespace Accounting_V4.Model.QNEModels.SupplierModels;

public class Creditors
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
    public Guid? ControlAccountId
    {
        get; set;
    }
    public Guid GLAccountId
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
    }
    public bool? IsProspect
    {
        get; set;
    }
    public int? CategoryId
    {
        get; set;
    }
    public string DeliveryAddress1
    {
        get; set;
    }
    public string DeliveryAddress2
    {
        get; set;
    }
    public string DeliveryAddress3
    {
        get; set;
    }
    public string DeliveryAddress4
    {
        get; set;
    }
    public int? AreaId
    {
        get; set;
    }
    public int? PurchaserId
    {
        get; set;
    }
    public int CurrencyId { get; set; } = 1;
    public int? TermId
    {
        get; set;
    }
    public decimal CreditLimit { get; set; } = 0.00000000M;
    public bool IsExceedCreditAllowed { get; set; } = false;
    public bool IsTaxExempted { get; set; } = false;
    public int StatementTypeId { get; set; } = 2;
    public int BillingCycleId { get; set; } = 2;
    public int StatementCycleId { get; set; } = 2;
    public string BillingContactPerson
    {
        get; set;
    }
    public string BillingContactPhoneNo
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
    public string AccountContactPhoneNo
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
    public string ManagementContactPhoneNo
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
    public string GuarantorGender { get; set; } = "M";
    public string GuarantorRace
    {
        get; set;
    }
    public string GuarantorCitizenship
    {
        get; set;
    }
    public decimal CurrentBalance { get; set; } = 0.00M;
    public int? GSTTypeId
    {
        get; set;
    }
    public string GSTExemptionNo
    {
        get; set;
    }
    public DateTime? GSTExemptionExpiryDate
    {
        get; set;
    }
    public int PurchasesTypeId { get; set; } = 0;
    public DateTime StartDate { get; set; } = DateTime.Now.Date;
    public string SMSGroupCode
    {
        get; set;
    }
    public string SMSNumber
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
    public int? PriceGroupId
    {
        get; set;
    }
    public DateTime? GSTStatusVerifiedDate
    {
        get; set;
    }
    public string Notes
    {
        get; set;
    }
    public int OptimisticLockField { get; set; } = 0;
    public Guid? BIRClassification
    {
        get; set;
    }
    public Guid? DefaultWTaxCodeId
    {
        get; set;
    }
    public string BranchCode { get; set; } = "0000";
    public Guid? PostingAccountGroupId
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
    public bool IsDeferredVAT { get; set; } = false;
    public string ZipCode
    {
        get; set;
    }
    public int? GroupId
    {
        get; set;
    }
    public string Status { get; set; } = "ACTIVE";
    public int? Maybank2ePaymentMethodId
    {
        get; set;
    }
    public int? PaymentBankId
    {
        get; set;
    }
    public string PaymentBankAccNo
    {
        get; set;
    }
    public int? PaymentNewId
    {
        get; set;
    }
    public int? PaymentOldId
    {
        get; set;
    }
    public int? PaymentOtherId
    {
        get; set;
    }
    public string PaymentBillerCode
    {
        get; set;
    }
    public string PaymentReference1
    {
        get; set;
    }
    public string PaymentReference2
    {
        get; set;
    }
    public string SalesTaxExemptionNo
    {
        get; set;
    }
    public DateTime? SalesTaxExemptionExpDate
    {
        get; set;
    }
}
