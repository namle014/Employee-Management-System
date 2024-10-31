using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Entities
{
    [Table("BenefitType")]

    public class BenefitType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public virtual ICollection<Benefit> Benefits { get; set; } = new List<Benefit>();

    }
}
