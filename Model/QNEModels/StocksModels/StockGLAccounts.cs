namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockGLAccounts
    {
        public Guid StockId
        {
            get; set;
        }
        public string StockCode
        {
            get; set;
        }
        public string StockName
        {
            get; set;
        }
        public Guid StockAccountPresetId
        {
            get; set;
        }
        public Guid SalesAccountId
        {
            get; set;
        }
        public Guid SalesReturnAccountId
        {
            get; set;
        }
        public Guid CashSalesAccountId
        {
            get; set;
        }
        public Guid PurchaseAccountId
        {
            get; set;
        }
        public Guid PurchaseReturnAccountId
        {
            get; set;
        }
    }
}
