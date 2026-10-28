namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockBundlesModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid StockId { get; set; }

        public Guid SubItemId { get; set; }

        public int Pos { get; set; }

        public string Description { get; set; }

        public int? ProjectId { get; set; }

        public Guid? StockLocationId { get; set; }

        public decimal Qty { get; set; } = 0.00000000M;

        public Guid? UOMId { get; set; }

        public decimal SalesPrice { get; set; } = 0.00000000M;

        public decimal PurchasesCost { get; set; } = 0.00000000M;

        public int? OptimisticLockField { get; set; }

        public decimal UnitPrice { get; set; } = 0.00000000M;

        public decimal UnitCost { get; set; } = 0.00000000M;

    }
}
