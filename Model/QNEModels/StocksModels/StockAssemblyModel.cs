namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockAssemblyModel
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
        public bool? IsCancelled { get; set; }
        public decimal? Cost { get; set; }
        public decimal? TotalCost { get; set; }
        public string AssemblyCode { get; set; }
        public DateTime AssemblyDate { get; set; } = DateTime.Now.Date;
        public string? TransferFromInfo { get; set; }
        public bool? UseStandardUnitCost { get; set; }
        public decimal? StandardUnitCost { get; set; }
        public Guid? StockBatchNumberId { get; set; }
        public int? StockType { get; set; }
        public string? SerialNumber { get; set; }
        public string? SerialNumberRef1 { get; set; }
        public string? SerialNumberRef2 { get; set; }
        public int? OptimisticLockField { get; set; }
        public bool? HasBOM { get; set; }
        public string? TransferFromMasterInfo { get; set; }

        public string? BatchNo { get; set; } // ADDED FOR STOCK ASSEMBLY LIST JULY 2, 2026

        public string? StockCode { get; set; } // ADDED FOR STOCK ASSEMBLY LIST JULY 2, 2026

        public string? FirstCreatedUser { get; set; } // ADDED FOR STOCK ASSEMBLY LIST JULY 2, 2026

        public DateTime? FirstCreatedDate { get; set; } // ADDED FOR STOCK ASSEMBLY LIST JULY 2, 2026

        public string? LastModifiedUser { get; set; } // ADDED FOR STOCK ASSEMBLY LIST JULY 2, 2026

        public DateTime? LastModifiedDate { get; set; } // ADDED FOR STOCK ASSEMBLY LIST JULY 2, 2026

        public int? PrintCount { get; set; } // ADDED FOR STOCK ASSEMBLY LIST JULY 2, 2026
    }
}
