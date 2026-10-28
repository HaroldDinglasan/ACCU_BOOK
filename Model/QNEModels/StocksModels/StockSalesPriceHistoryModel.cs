namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockSalesPriceHistoryModel
    {
        public Guid Oid { get; set; }
        public Guid Stock { get; set; }
        public Guid UOM { get; set; } // Unit of Measure
        public Guid Debtor { get; set; }
        public int PriceType { get; set; }
        public string DocCode { get; set; }
        public DateTime DocDate { get; set; }
        public decimal Price { get; set; }
        public string Discount { get; set; }
        public int OptimisticLockField { get; set; } = 0;
    }
}
