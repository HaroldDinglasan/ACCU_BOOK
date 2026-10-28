namespace Accounting_V4.Model.QNEModels.ForTransferModels
{
    public class StockOutTransferList
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public Guid? StockBatchNumberId { get; set; }

        public Guid? TransferLink { get; set; }

        public string? BatchNo { get; set; }

        public string StockOutCode { get; set; }

        public DateTime? FirstCreatedDate { get; set; }

        public DateTime? StockOutDate { get; set; }

        public string Description { get; set; }

        public string LastModifiedUser { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string ReferenceNo { get; set; }

        public decimal? TotalAmount { get; set; }

        public int? PrintCount { get; set; }
    }
}
