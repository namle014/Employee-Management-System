using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Entities
{
    public class EmploymentContract : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string ContractName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal BasicSalary { get; set; }
        public string? Clause { get; set; }
    }
}
