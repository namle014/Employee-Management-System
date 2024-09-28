using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Entities
{
    public class Salary : BaseEntity
    {
        public string UserId { get; set; }

        public DateTime Date { get; set; }

        public double PITax { get; set; }

        public double TotalSalary {  get; set; }
    }
}
