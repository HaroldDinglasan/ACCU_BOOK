namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockBOMModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? StockId { get; set; }

        public string? BOMCode { get; set; }

        public string? StockCode { get; set; }

        public string? Description { get; set; }

        public bool? IsDefault { get; set; }

        public decimal? AssemblyCost { get; set; }

        public Guid? StockLocationId { get; set; }

        public decimal? UnitCost { get; set; }

        public int? OptimisticLockField { get; set; }

        public string? Notes { get; set; }
    }
}
