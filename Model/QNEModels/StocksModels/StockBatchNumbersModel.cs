namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockBatchNumbersModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? StockId { get; set; }

        public string BatchNo { get; set; }

        public string Description { get; set; }

        public int? Pos { get; set; }

        public DateTime BatchNoManufacturingDate { get; set; }

        public DateTime BatchNoExpiryDate { get; set; }

        public Decimal? BalanceQty { get; set; }

        public int? OptimisticLockField { get; set; }

        public string BatchNoRef1 { get; set; }

        public string BatchNoRef2 { get; set; }

        public string StockCode { get; set; } // ADDED 

        public string LocationCode { get; set; } // ADDED JUNE 24, 2026

        public string UOMCode { get; set; } // ADDED JUNE 24, 2026

        public Guid? StockLocationId { get; set; } // ADDED JUNE 25, 2026

        public Guid? UOMId { get; set; } // ADDED JUNE 25, 2026
    }
}
