using System.ComponentModel.DataAnnotations;

namespace Accounting_V4.Model.UserModels;

public class UserModel 
{
    [Key]

    public Guid Oid
    {
        get; set;
    }

    [Required(ErrorMessage = "Password is Required")]
    public string StoredPassword
    {
        get; set;
    }

    [Required(ErrorMessage = "OldPassword is Required")]
    public string OldStoredPassword
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




    //public int Id
    //{
    //    get; set;
    //}

    //[Required(ErrorMessage = "Username is Required")]
    //public string Username
    //{
    //    get; set;
    //}

    //public string Password { get; set; } = "Defaultpassword";

    ////[Required(ErrorMessage = "Name is Required"), MaxLength(150, ErrorMessage = "You have reached maximum limit of characters allowed (150)")]
    //public string Name
    //{
    //    get; set;
    //}

    //public bool IsActive { get; set; } = true;

    //public string AccessType { get; set; } = "Grouped";

    //[Required]
    //[DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email.")]
    //[EmailAddress]
    //public string UserEmail { get; set; } = "ysonmencidorj@gmail.com";
}
