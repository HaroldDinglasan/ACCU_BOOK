namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class VW_StocksListing
    {
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string ClassCode { get; set; }
        public string ClassName { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public Guid Id { get; set; }
        public string StockCode { get; set; }
        public string StockName { get; set; }
        public string StockName2 { get; set; }
        public string Description { get; set; }
        public string BaseUOM { get; set; }
        public decimal MinQty { get; set; }
        public decimal MaxQty { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal ReorderQty { get; set; }
        public decimal SalesTax { get; set; }
        public decimal OpeningQty { get; set; }
        public decimal OpeningCost { get; set; }
        public decimal ListPrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal LastSellingPrice { get; set; }
        public string SalesDiscount { get; set; }
        public decimal PurchasePrice { get; set; }
        public string PurchaseDiscount { get; set; }
        public decimal LastPurchasePrice { get; set; }
        public string BarCode { get; set; }
        public decimal Weight { get; set; }
        public string WeightUOM { get; set; }
        public string Notes { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public DateTime? LastSellingDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsBundled { get; set; }
        public decimal LastPurchasePriceWithLandingCost { get; set; }
        public DateTime CreateDate { get; set; }
        public bool StockControl { get; set; }
        public string ItemTypeCode { get; set; }
        public string FurtherDescription { get; set; }
        public bool UseSerialNo { get; set; }
        public string SerialNoPrefix { get; set; }
        public string SerialNoSuffix { get; set; }
        public string Remark1 { get; set; }
        public string Remark2 { get; set; }
        public string Remark3 { get; set; }
        public string Remark4 { get; set; }
        public string Remark5 { get; set; }
        public bool UseBatchNo { get; set; }
        public int RowIndex { get; set; }
        public int ObjectType { get; set; }
    }
}
