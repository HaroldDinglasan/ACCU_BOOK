namespace Accounting_V4.Model.QNEModels.AllTaxCodeModels;

public class TaxCodeFilter
{
    public bool IsOutOrIn
    {
        get; set;
    }
    public bool? IsDefault
    {
        get; set;
    }
    public bool? IsActive
    {
        get; set;
    }
}
