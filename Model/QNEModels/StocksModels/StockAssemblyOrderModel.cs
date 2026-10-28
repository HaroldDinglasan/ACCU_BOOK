namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockAssemblyOrderModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? StockId { get; set; }
        public Guid? BOMId { get; set; }
        public Guid? UOMId { get; set; }
        public string? Description { get; set; }
        public decimal? Qty { get; set; } = 1.0000m;
        public string? ReferenceNo { get; set; }
        public int? ProjectId { get; set; }
        public Guid? StockLocationId { get; set; }
        public bool? IsCancelled { get; set; } = false;
        public decimal? Cost { get; set; } = 0.00m;
        public decimal? TotalCost { get; set; } = 0.00m;
        public string? AssemblyOrderCode { get; set; }
        public DateTime AssemblyOrderDate { get; set; } = DateTime.Now.Date;
        public bool? IsClosed { get; set; } = false;
        public string? TransferToInfo { get; set; }
        public Guid? StockBatchNumberId { get; set; }
        public int? StockType { get; set; }
        public string? SerialNumber { get; set; }
        public string? SerialNumberRef1 { get; set; }
        public string? SerialNumberRef2 { get; set; }
        public int? OptimisticLockField { get; set; } = 1;
        public bool? HasBOM { get; set; }
        public string? TransferToMasterInfo { get; set; }
        public decimal? TransferQty { get; set; } = 0m;
        public string? BatchNo { get; set; } // ADDED FOR LISTING STOCK ASSEBLY ORDER JUNE 29, 2026
    }
}
