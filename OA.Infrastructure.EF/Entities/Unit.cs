using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Entities
{
    public class Unit : BaseEntity
    {
        public string? Name { get; set; }
        public string? Note { get; set; }
        public string? UnitCode { get; set; }
    }
}
