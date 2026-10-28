namespace Accounting_V4.Model.QNEModels.DashBoardModels;

public class MothlySalesFilter
{
      public string AccountCode { get; set; }


    public int LocalOneNet
    {
        get; set;
    }
    public DateTime? DateFrom { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);

    public DateTime? DateTo { get; set; } = DateTime.Now;
}
