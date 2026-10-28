namespace Accounting_V4.Model.QNEModels.PuchasesModels
{
    public class GoodsReceivedNotesModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string GoodsReceivedNoteCode { get; set; }
        public DateTime GoodsReceivedNoteDate { get; set; } = DateTime.Now;
        public Guid? CreditorId { get; set; }
        public string? CreditorName { get; set; }
        public string? Attention { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? ReferenceNo { get; set; }
        public string? CreditorDONo { get; set; }
        public int? PurchaserId { get; set; }
        public int? TermId { get; set; }
        public string? DeliveryTerm { get; set; }
        public string? Title { get; set; }
        public string? Ref1 { get; set; }
        public string? Ref2 { get; set; }
        public string? Remark1 { get; set; }
        public string? Remark2 { get; set; }
        public string? DeliverContact { get; set; }
        public string? DeliverPhone { get; set; }
        public string? DeliverFax { get; set; }
        public int? ProjectId { get; set; }
        public int? DepartmentId { get; set; }
        public Guid? StockLocationId { get; set; }
        public int CurrencyId { get; set; }
        public decimal? CurrencyRate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalAmountLocal { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsClosed { get; set; }
        public int? CostCentreId { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? Address4 { get; set; }
        public string? DeliverAddress1 { get; set; }
        public string? DeliverAddress2 { get; set; }
        public string? DeliverAddress3 { get; set; }
        public string? DeliverAddress4 { get; set; }
        public string? Discount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountAmountLocal { get; set; }
        public decimal? NetTotalAmount { get; set; }
        public decimal? NetTotalAmountLocal { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? TaxTotalAmount { get; set; }
        public decimal? TaxTotalAmountLocal { get; set; }
        public decimal? SubtotalAmount { get; set; }
        public decimal? SubtotalAmountLocal { get; set; }
        public decimal? ImportDeclarationNo { get; set; }
        public string? Notes { get; set; }
        public bool IsTaxInclusive { get; set; }
        public DateTime? TaxDate { get; set; }
        public Decimal? TaxExclusiveTotalAmount { get; set; }
        public Decimal? TaxExclusiveTotalAmountLocal { get; set; }
        public bool? IsTaxInclusiveOnly { get; set; }
        public Guid? RoundingAdjustmentAccount { get; set; }
        public decimal? RoundingAdjustment { get; set; }
        public decimal? RoundingAdjustmentLocal { get; set; }
        public bool? IsRounding { get; set; }
        public int? OptimisticLockField { get; set; }
        public decimal? WTaxTotalAmount { get; set; }
        public decimal? WTaxTotalAmountLocal { get; set; }
        public decimal? SgstTotalAmount { get; set; }
        public decimal? SgstTotalAmountLocal { get; set; }
        public decimal? CgstTotalAmount { get; set; }
        public decimal? CgstTotalAmountLocal { get; set; }
        public decimal? IgstTotalAmount { get; set; }
        public decimal? IgstTotalAmountLocal { get; set; }
        public string? TransferFromInfo { get; set; }
        public string TransferToInfo { get; set; }
        public bool? IsApproved { get; set; }
        public decimal? ReverseRate { get; set; }
        public bool? IsNonTaxInvoice { get; set; }
        public bool? IsPostAccruedAP { get; set; }
        public bool? IsPast { get; set; }
        public bool? IsSelfBearWTax { get; set; }
        public decimal? TotalServiceAmount { get; set; }
        public decimal? TotalServiceAmountLocal { get; set; }
        public decimal? TotalAmountTaxExempted { get; set; }
        public decimal? TotalAmountTaxExemptedLocal { get; set; }
        public bool? IsEinvoiceReject { get; set; }
        public int? DimensionId { get; set; }

        public string CompanyCode { get; set; }

        public string CurrencyCode { get; set; }

        public string Name { get; set; }
    }
}
