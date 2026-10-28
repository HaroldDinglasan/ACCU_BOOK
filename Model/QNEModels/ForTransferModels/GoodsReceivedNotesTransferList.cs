namespace Accounting_V4.Model.QNEModels.ForTransferModels
{
    public class PurchaseOrderTransferList
    {
        public Guid Id { get; set; } // PO.Id

        public string DocCode { get; set; } // PurchaseOrderCode

        public DateTime? DocDate { get; set; } // PurchaseOrderDate 

        public Guid? CreditorId { get; set; } // Creditor Id

        public string CreditorName { get; set; } // Creditor Company Name

        public string Purchaser { get; set; } // Purchaser Name

        public string RequestBy { get; set; } // SYSR Username

        public string Term { get; set; }

        public decimal TaxTotalAmount { get; set; } = 0.00M;

        public decimal NetTotalAmount { get; set; } = 0.00M;

        public bool IsCancelled { get; set; }

        public bool IsClosed { get; set; }

        public bool IsApproved { get; set; }

    }
}
