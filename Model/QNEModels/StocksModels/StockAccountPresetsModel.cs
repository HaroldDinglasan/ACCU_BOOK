namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockAccountPresetsModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PresetCode { get; set; }
        public string Description { get; set; }
        public Guid? SalesAccountId { get; set; }
        public Guid? SalesReturnAccountId { get; set; }
        public Guid? CashSalesAccountId { get; set; }
        public Guid? PurchaseAccountId { get; set; }
        public Guid? PurchaseReturnAccountId { get; set; }
        public Guid? SalesDiscountAccountId { get; set; }
        public Guid? PurchaseDiscountAccountId { get; set; }
        public Guid? SalesDiscountReturnAccountId { get; set; }
        public Guid? PurchaseDiscountReturnAccountId { get; set; }
        public Guid? CostOfSalesAccountId { get; set; }
        public bool IsDefault { get; set; } = false;
        public int OptimisticLockField { get; set; } = 0;
        public int RowIndex { get; set; }
        public Guid? AccruedPurchaseAccountId { get; set; }

    }
}
