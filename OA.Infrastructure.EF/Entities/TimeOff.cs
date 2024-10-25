using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Entities
{
    public class TimeOff : BaseEntity
    {
        public string? Reason { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
      
    }
}
