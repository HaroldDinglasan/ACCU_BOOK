namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockInsModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string StockInCode
        {
            get; set;
        }

        public DateTime StockInDate { get; set; } = DateTime.Now.Date;

        public string Description
        {
            get; set;
        }

        public string ReferenceNo
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        public string Ref1
        {
            get; set;
        }

        public string Ref2
        {
            get; set;
        }

        public string Remark1
        {
            get; set;
        }

        public string Remark2
        {
            get; set;
        }

        public int? ProjectId
        {
            get; set;
        }

        public decimal TotalAmount
        {
            get; set;
        }

        public bool IsCancelled
        {
            get; set;
        }

        public Guid? StockLocationId
        {
            get; set;
        }

        public int? CostCentreId
        {
            get; set;
        }

        public string Notes
        {
            get; set;
        }

        public int? OptimisticLockField
        {
            get; set;
        }

        public bool? IsProduction
        {
            get; set;
        }

        public string? TransferFromInfo
        {
            get; set;
        }

        public string? Title2
        {
            get; set;
        }

        public int? DimensionId
        {
            get; set;
        }

        public string FirstCreatedUser
        {
            get; set;
        }

        public DateTime? FirstCreatedDate
        {
            get; set;
        }

        public string LastModifiedUser
        {
            get; set;
        }

        public DateTime? LastModifiedDate
        {
            get; set;
        }

        public int? PrintCount
        {
            get; set;
        }

    }
}
