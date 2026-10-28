namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockGroupsModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string GroupCode { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int OptimisticLockField { get; set; } = 0;
        public int RowIndex { get; set; }

    }
}
