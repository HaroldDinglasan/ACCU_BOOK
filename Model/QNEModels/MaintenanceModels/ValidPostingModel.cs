namespace Accounting_V4.Model.QNEModels.MaintenanceModels
{
    public class ValidPostingModel
    {
        public int Id { get; set; }

        public DateTime? ValidDateFrom { get; set; }

        public DateTime? ValidDateTo { get; set; }

        public bool? IsFirstLevel { get; set; }

        public bool? IsActive { get; set; }

        public string UserId { get; set; }

        public int? OptimisticLockField { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }   // 👈 Add this line

    }
}
