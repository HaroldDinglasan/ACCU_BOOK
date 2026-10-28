using Microsoft.Identity.Client;

namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockBOMDetailModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? BOMId { get; set; }

        public Guid? StockId { get; set; }

        public string? Description { get; set; }

        public Guid? UOMId { get; set; }

        public decimal? Qty { get; set; }

        public decimal? UnitCost { get; set; }

        public decimal? LabourCost { get; set; }

        public decimal? OverheadCost { get; set; }

        public Guid? StockLocationId { get; set; }

        public string? ItemTypeCode { get; set; }

        public int? Pos { get; set; }

        public int? OptimisticLockField { get; set; }

        public string? BOMCode { get; set; }

    }
}
