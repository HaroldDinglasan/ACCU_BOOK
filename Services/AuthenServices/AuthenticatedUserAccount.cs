
namespace Accounting_V4.Services.AuthenServices;

public class AuthenticatedUserAccount
{


    public Guid Oid
    {
        get; set;
    }
    public string StoredPassword
    {
        get; set;
    }
    public string UserName
    {
        get; set;
    }
    public string ChangePasswordOnFirstLogon
    {
        get; set;
    }
    public string FullName
    {
        get; set;
    }
    public string Remarks
    {
        get; set;
    }
    public byte[]? Picture
    {
        get; set;
    }
    public byte[]? Signature
    {
        get; set;
    }
    public string Email
    {
        get; set;
    }
    public string RoleType
    {
        get; set;
    }

    public string RoleName
    {
        get; set;
    }

    public Guid RoleId
    {
        get; set;
    }


    //public int Id
    //{
    //    get; set;
    //}
    //public string username
    //{
    //    get; set;
    //}
    //public string RoleName
    //{
    //    get; set;
    //}
    //public string Name
    //{
    //    get; set;
    //}
    //public string AccessType
    //{
    //    get; set;
    //}
    //public string Password
    //{
    //    get; set;
    //}
    //public bool IsPasswordDefault
    //{
    //    get; set;
    //}
}
