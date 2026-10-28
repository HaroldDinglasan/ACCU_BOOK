namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class NegativeBatchNoLine
    {
        public string StockCode { get; set; }

        public string BatchNo { get; set; }

        public decimal Available { get; set; }

        public decimal OutQty { get; set; }

        public decimal BatchNoBalance { get; set; }

        public string LocationCode { get; set; }
    }
}
