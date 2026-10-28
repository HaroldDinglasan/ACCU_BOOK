namespace Accounting_V4.Model.QNEModels.DashBoardModels;

public class TopOverdueCustomer
{
    public string CompanyCode
    {
        get; set;
    }
    public string CompanyName
    {
        get; set;
    }
    public decimal Outstanding { get; set; } = 0.0m;
    public decimal OutstandingLocal { get; set; } = 0.0m;
}
