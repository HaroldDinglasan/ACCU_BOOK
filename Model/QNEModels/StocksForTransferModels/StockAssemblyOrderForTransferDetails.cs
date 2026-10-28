namespace Accounting_V4.Model.QNEModels.StocksForTransferModels
{
    public class StockAssemblyOrderForTransferDetails
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SourceKey
        {
            get; set;
        }

        public decimal TransferredBaseUnit
        {
            get; set;
        }

        public decimal Transferred
        {
            get; set;
        }


        public Guid? StockAssemblyOrderDetailId
        {
            get; set;
        }

        public Guid? StockAssemblyDetailId
        {
            get; set;
        }

        public int OptimisticLockField { get; set; } = 1;

    }
}
