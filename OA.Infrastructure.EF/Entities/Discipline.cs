using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Entities
{
    public class Discipline : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Reason { get; set; }
        public double Money { get; set; }
        public string? Note { get; set; }
    }
}
