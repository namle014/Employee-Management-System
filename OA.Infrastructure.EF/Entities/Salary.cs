using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Entities
{
    public class Salary
    {
        public String Id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UserId { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public double PITax { get; set; }

        public double TotalSalary {  get; set; }
    }
}
