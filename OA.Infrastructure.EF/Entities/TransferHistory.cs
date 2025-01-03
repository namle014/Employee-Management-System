using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Entities
{
    public class TransferHistory
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } =string.Empty;
        public DateTime TransferDate { get; set; }
        public int FromDepartmentId { get; set; }
        public int ToDepartmentId { get; set; }
        public string? Reason { get; set; }
    }
}
