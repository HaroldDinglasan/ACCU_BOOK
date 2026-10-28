namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class VW_StockClassesListing
    {
        public Guid Id { get; set; }

        public string ClassCode { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int OptimisticLockField { get; set; }

        public int RowIndex { get; set; }

    }
}
