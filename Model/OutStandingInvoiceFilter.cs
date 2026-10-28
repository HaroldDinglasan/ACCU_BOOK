namespace Accounting_V4.Model
{
    public class OutStandingInvoiceFilter:BaseForm
    {
        public bool withPDCheques { get; set; } = true;
        public bool includeFutureKnockOff { get; set; } = false;

    }
}
