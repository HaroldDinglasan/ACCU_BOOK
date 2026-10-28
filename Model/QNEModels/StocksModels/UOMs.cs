using System.ComponentModel.DataAnnotations;

namespace Accounting_V4.Model.QNEModels.StocksModels;

public class UOMs
{
   
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid StockId
    {
        get; set;
    }

    public string UOMCode
    {
        get; set;
    }

    public string Description
    {
        get; set;
    }
    


    public decimal SalesPrice { get; set; } = 0.00000000M;


    public decimal Rate { get; set; } = 1.0000M;

    public bool IsBaseUOM { get; set; } = false;


    public decimal PurchasePrice { get; set; } = 0.00000000M;


    public decimal MinSalesPrice { get; set; } = 0.00000000M;


    public decimal MaxSalesPrice { get; set; } = 0.00000000M;


    public decimal MinPurchasePrice { get; set; } = 0.00000000M;


    public decimal MaxPurchasePrice { get; set; } = 0.00000000M;


    public string SalesDiscount
    {
        get; set;
    }


    public string PurchaseDiscount
    {
        get; set;
    }


    public string BarCode
    {
        get; set;
    }

    public int Pos
    {
        get; set;
    }

    public bool IsActive { get; set; } = true;


    public int OptimisticLockField { get; set; } = 0;

    public bool IsBatchBaseUOM { get; set; } = false;

    public bool IsECProduct { get; set; } = false;
}
