using Accounting_V4.Model.QNEModels.TransactionCodesModels;

namespace Accounting_V4.Model.QNEModels.StocksModels
{
    public class StockOutDetails : StockTransDetail
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid StockOutId { get; set; }

        public int Pos { get; set; }

        public string? Numbering { get; set; }

        public Guid? StockId { get; set; }

        public string? Description { get; set; }

        public Guid? UOMId { get; set; }

        public decimal? Price { get; set; }


        private decimal? _unitPrice;

        public decimal? UnitPrice
        {
            get => _unitPrice ?? Price;
            set => _unitPrice = value;
        }

        public decimal? Qty { get; set; }

        public decimal? Amount { get; set; }

        public int? ProjectId { get; set; }

        public Guid? StockLocationId { get; set; }

        public int? CostCentreId { get; set; }

        public string? Note { get; set; }

        public string? ItemTypeCode { get; set; }

        public int? StockType { get; set; }

        public int? OptimisticLockField { get; set; }

        public Guid? StockBatchNumberId { get; set; }

        public decimal Transferred { get; set; }

        public bool? IsBundled { get; set; }

        public bool? IsSubItem { get; set; }

        public Guid? BundledTransactionId { get; set; }

        public string? SerialNumber { get; set; }

        public string? SerialNumberRef1 { get; set; }

        public string? SerialNumberRef2 { get; set; }

        public Guid? PackingUOMId { get; set; }

        public decimal? Packing { get; set; }

        public decimal? PackingQty { get; set; }

        public string? Ref1 { get; set; }

        public string? Ref2 { get; set; }

        public string? Ref3 { get; set; }

        public string? Ref4 { get; set; }

        public string? Ref5 { get; set; }

        public decimal? PackingUOM1 { get; set; }

        public decimal? PackingUOM2 { get; set; }

        public string? HighestUOM { get; set; }

        public int? DimensionId { get; set; }

        public string? BatchNo { get; set; }
    }
}
