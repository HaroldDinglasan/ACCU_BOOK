namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class NegativeInventoryLine
    {
        public string StockName { get; set; }

        public string StockCode { get; set; }

        public string LocationCode { get; set; }

        public decimal OnHand { get; set; }

        public decimal OutQty { get; set; }

        public decimal StockBalance { get; set; }
    }
}
