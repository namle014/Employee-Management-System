using OA.Core.Constants;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OA.Core.VModels
{
    public class CreatePromotionHistory
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string FromRoleId { get; set; } = string.Empty;
        public string ToRoleId { get; set; } = string.Empty;
        public decimal? ToSalary { get; set; }
    }

    public class UpdatePromotionHistory : CreatePromotionHistory
    {
        public int Id { get; set; }
    }

    public class GetAllPromotionHistory : UpdatePromotionHistory
    {
        public DateTime PromotionDate { get; set; }
        public string? Reason { get; set; }
        public decimal? FromSalary { get; set; }

    }

    public class GetByIdPromotionHistory : GetAllPromotionHistory {
        public string FromRoleName { get; set; } = string.Empty;
        public string ToRoleName { get; set; } = string.Empty;

    }
}
