namespace Accounting_V4.Model.QNEModels.AllTaxCodeModels
{
    public class FilterTaxCodeResult
    {
        public List<TaxCodes> InputTaxCodes { get; set; } = new List<TaxCodes>();
        public List<TaxCodes> OutputTaxCodes { get; set; } = new List<TaxCodes>();

    }
}
