using System.ComponentModel.DataAnnotations.Schema;

namespace OA.Infrastructure.EF.Entities
{
    public class Insurance
    {
        public string Id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Name { get; set; } = string.Empty;
        public int InsuranceTypeId { get; set; }
        public decimal InsuranceContribution { get; set; }
        [ForeignKey("InsuranceTypeId")] 
        public virtual required InsuranceType InsuranceType { get; set; } 

    }
}
