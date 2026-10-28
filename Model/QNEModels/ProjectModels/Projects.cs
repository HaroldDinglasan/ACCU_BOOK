namespace Accounting_V4.Model.QNEModels.ProjectModels;

public class Projects
{
    public int Id
    {
        get; set;
    }
    public string ProjectCode
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
    public int? ParentId
    {
        get; set;
    }
    public DateTime? StartDate
    {
        get; set;
    }
    public DateTime? ProjectedEndDate
    {
        get; set;
    }
    public DateTime? EndDate
    {
        get; set;
    }
    public decimal ContractSum { get; set; } = 0.00m;
    public bool IsActive { get; set; } = true;
    public int? DivisionId
    {
        get; set;
    }
    public int OptimisticLockField { get; set; } = 0;
    public string Header
    {
        get; set;
    }
    public string CompanyName
    {
        get; set;
    }
    public string CompanyName2
    {
        get; set;
    }
    public string RegNo
    {
        get; set;
    }
    public string Address1
    {
        get; set;
    }
    public string Address2
    {
        get; set;
    }
    public string Address3
    {
        get; set;
    }
    public string Address4
    {
        get; set;
    }
    public string Address5
    {
        get; set;
    }
    public string TradeName
    {
        get; set;
    }
    public string ZipCode
    {
        get; set;
    }
    public string ContactPerson
    {
        get; set;
    }
    public string Phone1
    {
        get; set;
    }
    public string Phone2
    {
        get; set;
    }
    public string GSTRegNo
    {
        get; set;
    }
    public string Fax1
    {
        get; set;
    }
    public string Fax2
    {
        get; set;
    }
    public string Email
    {
        get; set;
    }
    public string HomePage
    {
        get; set;
    }
    public string BusinessNature
    {
        get; set;
    }
    public string Signatory
    {
        get; set;
    }
    public string GSTApprovalNo
    {
        get; set;
    }
    public byte?[] Picture
    {
        get; set;
    }
    public Guid? CBIRClassification
    {
        get; set;
    }
    public Guid? RDOData
    {
        get; set;
    }
}
