using System.ComponentModel.DataAnnotations;

namespace Accounting_V4.Model.QNEModels.AllTaxCodeModels;

public class GLAccountTaxCodeLinks
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? GLAccountId
    {
        get; set;
    }
    public Guid? DefaultInputTaxCodeId
    {
        get; set;
    }
    public Guid? DefaultOutputTaxCodeId
    {
        get; set;
    }
    public int OptimisticLockField { get; set; } = 0;
}
