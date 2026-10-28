using System.ComponentModel.DataAnnotations;

namespace Accounting_V4.Model.QNEModels.AllTaxCodeModels;

public class DefaultTaxCode
{
    public Guid Id
    {
        get; set;
    }
    public Guid TaxTypeId
    {
        get; set;
    }
    public string TaxCode
    {
        get; set;
    }
    public string VatType
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
    public string Rate
    {
        get; set;
    }
    public decimal TaxRate
    {
        get; set;
    }
    public bool IsOutputTax
    {
        get; set;
    }
    public int TaxDescription
    {
        get; set;
    }
    public int TaxDescriptionPH
    {
        get; set;
    }
    public int TaxDescriptionLK
    {
        get; set;
    }
    public bool IsTaxInclusive
    {
        get; set;
    }
    public Guid GLAccountId
    {
        get; set;
    }
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; } = false;
    public bool IsSystemData { get; set; } = true;
    public int TaxClass
    {
        get; set;
    }
    public Guid InputTaxCodeId
    {
        get; set;
    }
    public Guid OutputTaxCodeId
    {
        get; set;
    }
    public decimal MaximumPurchaseAmount
    {
        get; set;
    }
    public decimal MaximumTaxAmount
    {
        get; set;
    }
    public string TaxClassValue
    {
        get; set;
    }
    public Guid GSTGLAccountId
    {
        get; set;
    }
    public bool PostGSTExpenseToSameAccount { get; set; } = false;
    public int OptimisticLockField { get; set; } = 0;
    public int TaxDescriptionIN { get; set; } = 0;

    public bool UseSGST { get; set; } = false;
    public bool UseCGST { get; set; } = false;
    public bool UseIGST { get; set; } = false;

    [DisplayFormat(DataFormatString = "{0:0.00}")]
    public decimal SGST
    {
        get; set;
    }
    [DisplayFormat(DataFormatString = "{0:0.00}")]
    public decimal CGST
    {
        get; set;
    }
    [DisplayFormat(DataFormatString = "{0:0.00}")]
    public decimal IGST
    {
        get; set;
    }
    public Guid SgstPostingAccount
    {
        get; set;
    }
    public Guid CgstPostingAccount
    {
        get; set;
    }
    public Guid IgstPostingAccount
    {
        get; set;
    }
    public int TaxDescriptionKH { get; set; } = 0;
    public int TaxDescriptionMV { get; set; } = 0;
    public int TaxDescriptionSG { get; set; } = 0;
    public Guid GLAccountDeferredVatId
    {
        get; set;
    }
    public Guid GLAccountServicePaymentId
    {
        get; set;
    }
    public Guid GLAccountSelfRecipientId
    {
        get; set;
    }

    [DisplayFormat(DataFormatString = "{0:0.00}")]
    public decimal TaxRateSelfRecipient
    {
        get; set;
    }

}
