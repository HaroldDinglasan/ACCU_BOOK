namespace Accounting_V4.Model.QNEModels.PuchasesModels;

public class ForCloseCancelledDocumentsModel
{
    public Guid? Id
    {
        get; set;
    }
    public bool IsClosed
    {
        get; set;
    } 
    public bool IsCancelled
    {
        get; set;
    }
    public string TableName
    {
        get; set;
    }
    public string SwitchName
    {
        get; set;
    }
}
