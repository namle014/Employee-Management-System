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
        public string UserId { get; set; } = string.Empty;
        public string BenefitId { get; set; } = string.Empty;
    }

    
}
