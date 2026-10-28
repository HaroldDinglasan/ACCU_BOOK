namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockAssemblyOrderDetail
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Numbering { get; set; }
        public Guid? StockId { get; set; }
        public string? Description { get; set; }
        public Guid? UOMId { get; set; }
        public decimal? Qty { get; set; }
        public decimal? DefaultQty { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? LabourCost { get; set; }
        public decimal? OverheadCost { get; set; }
        public Guid? StockLocationId { get; set; }
        public int? ProjectId { get; set; }
        public Guid? StockAssemblyOrderId { get; set; }
        public int? Pos { get; set; }
        public string? Ref1 { get; set; }
        public string? Ref2 { get; set; }
        public string? Ref3 { get; set; }
        public string? Ref4 { get; set; }
        public string? Ref5 { get; set; }
        public decimal? Transferred { get; set; }
        public int? StockType { get; set; }
        public string? SerialNumber { get; set; }
        public string? SerialNumberRef1 { get; set; }
        public string? SerialNumberRef2 { get; set; }
        public Guid? StockBatchNumberId { get; set; }
        public int? OptimisticLockField { get; set; }
        public Guid? TransferLink { get; set; }

        public string? BatchNoRef1 { get; set; } // Added Aug 4, 2026 (Read only fields)

        public string? BatchNoRef2 { get; set; } // Added Aug 4, 2026 (Read only fields)

        public DateTime? BatchNoExpiryDate { get; set; } // Added Aug 4, 2026 (Read only fields)

    }
}
