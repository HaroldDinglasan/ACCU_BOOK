using Accounting_V4.Services.QNEServices.TransferServices;
using Accounting_V4.Services.SystemServices.TimeZoneServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Accounting_V4.Model.QNEModels.StocksModels;

public class Stocks
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string StockCode
    {
        get; set;
    }
    public Guid? ItemTypeId
    {
        get; set;
    }
    public string StockName
    {
        get; set;
    }
    public string StockName2
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
    public Guid? CategoryId
    {
        get; set;
    }
    public Guid? ClassId
    {
        get; set;
    }
    public Guid? GroupId
    {
        get; set;
    }
    public string BaseUOM { get; set; } = "PC";
    public decimal MinQty { get; set; } = 0.00000000M;
    public decimal MaxQty { get; set; } = 0.00000000M;
    public decimal ReorderLevel { get; set; } = 0.00000000M;
    public decimal ReorderQty { get; set; } = 0.00000000M;
    public decimal? SalesTax
    {
        get; set;
    }
    public Guid? SalesTaxGLAccountId
    {
        get; set;
    }
    public decimal? OpeningQty
    {
        get; set;
    }
    public decimal? OpeningCost
    {
        get; set;
    }
    public decimal ListPrice { get; set; } = 0.00000000M;
    public decimal MinPrice { get; set; } = 0.00000000M;
    public decimal? LastSellingPrice
    {
        get; set;
    }
    public string SalesDiscount
    {
        get; set;
    }
    public decimal PurchasePrice { get; set; } = 0.00000000M;
    public string PurchaseDiscount
    {
        get; set;
    }
    public decimal? LastPurchasePrice
    {
        get; set;
    }
    public string BarCode
    {
        get; set;
    }
    public decimal Weight { get; set; } = 0.00000000M;
    public string WeightUOM
    {
        get; set;
    }
    public decimal Volumn { get; set; } = 0.00000000M;
    public string VolumnUOM
    {
        get; set;
    }
    public Guid? DefaultLocationId
    {
        get; set;
    }
    public byte?[] Picture
    {
        get; set;
    }
    public string Notes
    {
        get; set;
    }
    public decimal CurrentBalance { get; set; } = 0.00000000M;
    public DateTime? LastSellingDate
    {
        get; set;
    }
    public DateTime? LastPurchaseDate
    {
        get; set;
    }
    public bool IsActive { get; set; } = true;
    public bool IsBundled { get; set; } = false;
    public decimal? LastPurchasePriceWithLandingCost
    {
        get; set;
    }
    public Guid? StockAccountPresetId
    {
        get; set;
    }
  
    public DateTime CreateDate
    {
        get; set;
    }  = TimeService.ServerTime;
    public bool StockControl { get; set; } 
    public string ItemTypeCode
    {
        get; set;
    }
    public string FurtherDescription
    {
        get; set;
    }
    public Guid? DefaultInputTaxCodeId
    {
        get; set;
    }
    public Guid? DefaultOutputTaxCodeId
    {
        get; set;
    }
    public bool UseSerialNo { get; set; } = false;
    public string SerialNoPrefix
    {
        get; set;
    }
    public string SerialNoSuffix
    {
        get; set;
    }
    public int OptimisticLockField { get; set; } = 0;
    public int ObjectType
    {
        get; set;
    } 
    public string Remark1
    {
        get; set;
    }
    public string Remark2
    {
        get; set;
    }
    public string Remark3
    {
        get; set;
    }
    public string Remark4
    {
        get; set;
    }
    public string Remark5
    {
        get; set;
    }
    public string HSN
    {
        get; set;
    }
    public bool UseBatchNo { get; set; } = true;
    public bool UseDefaultUnsalablePeriod { get; set; } = true;
    public int UnsalablePeriod { get; set; } = 0;
    public int RowIndex
    {
        get; set;
    }
    public int? CommissionGroupId
    {
        get; set;
    }
    public bool AllowNegativeStock { get; set; } = false;
    public string SSTTariffCode
    {
        get; set;
    }
    public int? TariffCodeId
    {
        get; set;
    }
    public bool DiscountApplyToQty { get; set; } = false;
    public decimal FloorPrice { get; set; } = 0.00000000M;
    public string DisplayInModules
    {
        get; set;
    }
    public decimal PointMgtQty { get; set; } = 0.00M;
    public decimal PointMgtPointsPerQty { get; set; } = 0.00M;

    public int? ClassificationId { get; set; }

    public string? MarkupDiscountPrice { get; set; }

    public int? EInvoiceValidation { get; set; }
}
