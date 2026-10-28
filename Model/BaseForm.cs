namespace Accounting_V4.Model
{
    public class BaseForm
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

    }
}
