namespace Accounting_V4.Model.QNEModels.StocksModels;

public class StockUnitPrice
{
    public Guid? StockId
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
    public decimal UnitPrice { get; set; } = 0.00000000M;
}
