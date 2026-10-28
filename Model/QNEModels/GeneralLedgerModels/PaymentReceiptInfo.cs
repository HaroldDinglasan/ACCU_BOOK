namespace Accounting_V4.Model.QNEModels.GeneralLedgerModels;

public class PaymentReceiptInfo
{
    public Guid Id
    {
        get; set;
    } = Guid.NewGuid();

    public int? TableType
    {
        get; set;
    }
    public string? Description
    {
        get; set;
    }
    public string? RegistrationNo
    {
        get; set;
    }
    public string? GSTRegNo
    {
        get; set;
    }
    public string? Address
    {
        get; set;
    }
    public string? PhoneNo
    {
        get; set;
    }
    public bool IsDefault
    {
        get; set;
    } = false;
    public int OptimisticLockField
    {
        get; set;
    } = 0;
    public int RowIndex
    {
        get; set;
    } = 0;

}
