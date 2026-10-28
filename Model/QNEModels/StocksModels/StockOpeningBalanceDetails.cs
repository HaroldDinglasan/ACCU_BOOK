namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockOpeningBalanceDetails
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid StockOBId { get; set; }
        public Guid? StockId { get; set; }
        public int Pos { get; set; }
        public int? ProjectId { get; set; }
        public Guid? StockLocationId { get; set; }
        public Guid? UOMId { get; set; }
        public decimal Qty { get; set; } = 0.00000000M;
        public decimal? Price { get; set; } = 0.00000000M;
        public string Description { get; set; }
        public decimal Amount { get; set; } = 0.00M;
        public string Note { get; set; }
        public string ItemTypeCode { get; set; }
        public int? CostCentreId { get; set; }
        public string Numbering { get; set; }
        public int? StockType { get; set; }
        public int? OptimisticLockField { get; set; }
        public Guid? StockBatchNumberId { get; set; }
        public bool? IsBundled { get; set; }
        public bool? IsSubItem { get; set; }
        public Guid? BundledTransactionId { get; set; }
        public string SerialNumber { get; set; }
        public string SerialNumberRef1 { get; set; }
        public string SerialNumberRef2 { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Ref3 { get; set; }
        public string Ref4 { get; set; }
        public string Ref5 { get; set; }
        public DateTime StockAgingDate { get; set; }
    }
}
