using System.ComponentModel.DataAnnotations.Schema;

namespace OA.Infrastructure.EF.Entities
{
    public class PromotionHistory
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public DateTime PromotionDate { get; set; }
        public string FromRoleId { get; set; } = string.Empty;
        public string ToRoleId { get; set;} = string.Empty;
        public decimal? FromSalary { get; set; } 
        public decimal? ToSalary { get; set;} 
        public string? Reason { get; set; }

    }
}
