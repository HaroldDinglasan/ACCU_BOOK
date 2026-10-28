namespace Accounting_V4.Model.QNEModels.StocksForTransferModels
{
    public class StockAssemblyOrderForTransferHeader
    {
        public Guid Id
        {
            get; set;
        }

        public Guid? StockAssemblyOrderHeaderId
        {
            get; set;
        }

        public Guid? StockAssemblyHeaderId
        {
            get; set;
        }

        public string? TransferToInfo
        {
            get; set;
        }

        public string? TransferToMasterInfo
        {
            get; set;
        }

        public decimal? TransferQty
        {
            get; set;
        }

    }
}
