namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class VW_StockCategoriesListing
    {
        public Guid Id { get; set; }

        public string CategoryCode { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int OptimisticLockField { get; set; } = 0;

        public int RowIndex { get; set; }

    }
}
