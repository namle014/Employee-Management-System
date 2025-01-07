using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Entities
{
    public class BenefitUser
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string BenefitId { get; set; } = string.Empty;
        public decimal BenefitContribution { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsReceived { get; set; } = false;
    }

    
}
