namespace Accounting_V4.Model.QNEModels.ForTransferModels
{
    public class StockAssemblyOrderTransferListModel
    {
        public Guid Id { get; set; }

        public DateTime AssemblyOrderDate { get; set; }

        public string? AssemblyOrderCode { get; set; }

        public string? Description { get; set; }

        public string? ReferenceNo { get; set; }

        public decimal? UnitCost { get; set; }

        public decimal? SAOQty { get; set; }

        public decimal? RemainingQty { get; set; }

        public int? OptimisticLockField { get; set; }

    }
}
