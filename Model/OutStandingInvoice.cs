using System.ComponentModel.DataAnnotations.Schema;

namespace Accounting_V4.Model
{
    public class OutStandingInvoice
    {
        [NotMapped]
        public int TotalCount { get; set; }


        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string StaffCode { get; set; }
        public string DocSalesPersonCode { get; set; }
        public string Term { get; set; }
        public int AreaCode { get; set; }
        public string AreaName { get; set; }
        public string CategoryCode { get; set; }
        public DateTime DueDate { get; set; }
        public string DocType { get; set; }
        public string DocumentCode { get; set; }
        public string Description { get; set; }
        public string DefaultTerm { get; set; }
        public string StaffName { get; set; }
        public decimal DetailAmt { get; set; }
        public decimal DetailTaxAmt { get; set; }
        public decimal DetailNetAmt { get; set; }
    }

}
