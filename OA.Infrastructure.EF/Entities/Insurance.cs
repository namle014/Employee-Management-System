using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Entities
{
    public class Insurance : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int InsuranceTypeId { get; set; }
        public double InsuranceContribution { get; set; }
    }
}
