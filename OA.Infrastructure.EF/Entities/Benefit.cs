using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Entities
{
    public class Benefit : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int BenefitTypeId { get; set; }
        public decimal? BenefitContribution { get; set; }
    }
}
