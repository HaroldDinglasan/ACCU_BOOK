namespace Accounting_V4.Model.QNEModels.SupplierModels;

public class DefaultVatCode
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
    public string TaxRate
    {
        get; set;
    }
    public string TaxType
    {
        get; set;
    }
    public bool IsOutputTax
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
}
