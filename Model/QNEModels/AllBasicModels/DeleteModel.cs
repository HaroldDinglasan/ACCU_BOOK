namespace Accounting_V4.Model.QNEModels.AllBasicModels;

public class DeleteModel
{
    public Guid Id
    {
        get; set;
    }
    public Guid DetailsId
    {
        get; set;
    }
    public string TableName
    {
        get; set;
    }

    public decimal Amount
    {
        get; set;
    }

    public bool DeleteDetailRow
    {
        get; set;
    } = false;
}
