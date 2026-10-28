namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class VW_StockAccountPresetListing
    {
        public Guid Id { get; set; }
        public string PresetCode { get; set; }
        public string Description { get; set; }
        public Guid SalesAccountId { get; set; }
        public Guid SalesReturnAccountId { get; set; }
        public Guid CashSalesAccountId { get; set; }
        public Guid PurchaseAccountId { get; set; }
        public Guid PurchaseReturnAccountId { get; set; }
        public Guid SalesDiscountAccountId { get; set; }
        public Guid PurchaseDiscountAccountId { get; set; }
        public Guid SalesDiscountReturnAccountId { get; set; }
        public Guid PurchaseDiscountReturnAccountId { get; set; }
        public Guid CostOfSalesAccountId { get; set; }
        public bool IsDefault { get; set; } = false;
        public int OptimisticLockField { get; set; }
        public int RowIndex { get; set; }
        public Guid AccruedPurchaseAccountId { get; set; }
        public string SalesAccountCode { get; set; }
        public string SalesAccountDescription { get; set; }
        public string SalesReturnAccountCode { get; set; }
        public string SalesReturnAccountDescription { get; set; }
        public string CashSalesAccountCode { get; set; }
        public string CashSalesAccountDescription { get; set; }
        public string PurchaseAccountCode { get; set; }
        public string PurchaseAccountDescription { get; set; }
        public string PurchaseReturnAccountCode { get; set; }
        public string PurchaseReturnAccountDescription { get; set; }
        public string SalesDiscountAccountCode { get; set; }
        public string SalesDiscountAccountDescription { get; set; }
        public string PurchaseDiscountAccountCode { get; set; }
        public string PurchaseDiscountAccountDescription { get; set; }
        public string SalesDiscountReturnAccountCode { get; set; }
        public string SalesDiscountReturnAccountDescription { get; set; }
        public string PurchaseDiscountReturnAccountCode { get; set; }
        public string PurchaseDiscountReturnAccountDescription { get; set; }
        public string CostOfSalesAccountCode { get; set; }
        public string CostOfSalesAccountDescription { get; set; }
    }
}
