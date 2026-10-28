namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockLocationsModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string LocationCode { get; set; }
        public string Description { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Contact { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public bool IsActive { get; set; } = true;
        public int OptimisticLockField { get; set; } = 0;
        public int RowIndex { get; set; }
    }
}
