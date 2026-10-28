namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockClassesModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ClassCode { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int OptimisticLockField { get; set; }
        public int RowIndex { get; set; }

    }
}
