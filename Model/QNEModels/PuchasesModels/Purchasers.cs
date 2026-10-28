namespace Accounting_V4.Model.QNEModels.PuchasesModels;

public class Purchasers
{
    public int Id
    {
        get; set;
    }
    public string StaffCode
    {
        get; set;
    }
    public string Name
    {
        get; set;
    }
    public string IDNo
    {
        get; set;
    }
    public DateTime? DateJoined
    {
        get; set;
    }
    public DateTime? DateLeft
    {
        get; set;
    }
    public string MobileNo
    {
        get; set;
    }
    public string Email
    {
        get; set;
    }
    public decimal Salary { get; set; } = 0.00m;
    public string SOCSO
    {
        get; set;
    }
    public string EPF
    {
        get; set;
    }
    public string TaxFile
    {
        get; set;
    }
    public string Remarks
    {
        get; set;
    }
    public string Gender
    {
        get; set;
    } = "M";
    public bool IsActive { get; set; } = true;
    public bool IsManager { get; set; } = false;
    public string DefaultTeam
    {
        get; set;
    }
    public bool IsDefault { get; set; } = false;
    public int OptimisticLockField { get; set; } = 0;
}
