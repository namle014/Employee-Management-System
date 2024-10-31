using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Entities
{
    public class Benefit
    {
        public string Id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Name { get; set; } = string.Empty;
        public int BenefitTypeId { get; set; }
        public decimal BenefitContribution { get; set; }
        [ForeignKey("BenefitTypeId")]
        public virtual required BenefitType BenefitType { get; set; }
    }
}
