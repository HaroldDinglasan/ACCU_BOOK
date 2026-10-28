namespace Accounting_V4.Model.QNEModels.SalesModels;

public class SalesTargetCommissionRate
{
    public int SalesPersonId { get; set; }
    public string SalesTargetDescription { get; set; }
    public decimal CommissionRate { get; set; }
    public int Id { get; set; }
    public int Pos { get; set; }
}
