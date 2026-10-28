namespace Accounting_V4.Model.QNEModels.DashBoardModels;

public class TopStockReorder
{
    public string StockCode
    {
        get; set;
    }
    public string StockName
    {
        get; set;
    }
    public decimal MinQty
    {
        get; set;
    } = 0.0m;
    public decimal MaxQty
    {
        get; set;
    } = 0.0m;
    public decimal ReorderLevel
    {
        get; set;
    } = 0.0m;
    public decimal ReorderQty
    {
        get; set;
    } = 0.0m;

    public decimal CurrentBalance
    {
        get; set;
    } = 0.0m;
}
