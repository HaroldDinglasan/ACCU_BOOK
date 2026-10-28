namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockOpeningBalanceModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string StockOBCode { get; set; }
        public DateTime StockOBDate { get; set; }
        public string Description { get; set; }
        public string ReferenceNo { get; set; }
        public int? ProjectId { get; set; }
        public Guid? StockLocationId { get; set; }
        public string Notes { get; set; }
        public decimal TotalAmount { get; set; }
        public int? CostCentreId { get; set; }
        public int? OptimisticLockField { get; set; }
    }
}
