using System.ComponentModel.DataAnnotations;

namespace Accounting_V4.Model.MenuModels;

public class SystemMenuObjectPermission
{
    [Key]
    public Guid Oid { get; set; } = Guid.NewGuid(); // Primary Key
    public int MenuId
    {
        get; set;
    }                   // Links to SystemMenuObject.Id
    public int ParentMenuId
    {
        get; set;
    }             // Parent menu reference
    public string? PageName
    {
        get; set;
    }             // Page path
    public string MenuName
    {
        get; set;
    }              // Menu display name
    public string? IconNames
    {
        get; set;
    }            // Icon identifier

    // Permission flags
    public bool AllowNavigate { get; set; } = false;
    public bool AllowRead { get; set; } = false;
    public bool AllowWrite { get; set; } = false;
    public bool AllowCreate { get; set; } = false;
    public bool AllowDelete { get; set; } = false;
    public bool AllowExport { get; set; } = false;

    public Guid? Owner
    {
        get; set;
    }                  // FK to SystemRole.Oid

    public bool AllowPrint { get; set; } = false;
    public bool AllowCreditControl { get; set; } = false;
    public bool AllowChangeDocNumbering { get; set; } = false;
    public bool AllowChangeUnitCost { get; set; } = false;
    public bool AllowNegativeStockControl { get; set; } = false;
    public bool AllowStockReorderLevelControl { get; set; } = false;
    public bool AllowFilter { get; set; } = false;
    public bool AllowPreview { get; set; } = false;
    public bool AllowInquiry { get; set; } = false;
    public bool AllowDesign { get; set; } = false;
    public bool AllowSelectDocNumbering { get; set; } = false;
    public bool AllowChangeUnitPrice { get; set; } = false;
    public bool AllowEInvoiceValidation { get; set; } = false;
}
