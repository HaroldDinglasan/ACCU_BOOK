using System.ComponentModel.DataAnnotations;

namespace Accounting_V4.Model.QNEModels.StocksModels;

public class StockUOMLinks
{
    public int Id
    {
        get; set;
    }
    public Guid? StockId
    {
        get; set;
    }
    public Guid? SalesUOMId
    {
        get; set;
    }
    public Guid? PurchaseUOMId
    {
        get; set;
    }

    public int OptimisticLockField
    {
        get; set;
    } = 0;
    public Guid? SalesPackingUOMId
    {
        get; set;
    }
    public Guid? PurchasePackingUOMId
    {
        get; set;
    }
}
