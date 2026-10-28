namespace Accounting_V4.Model.QNEModels.DashBoardModels;

public class TopNearExpiryItem
{
    public string StockCode
    {
        get; set;
    }
    public string StockName
    {
        get; set;
    }
    public decimal Qty
    {
        get; set;
    } = 0.0m;

    public DateTime BatchNoExpiryDate
    {
        get; set;
    }
}
