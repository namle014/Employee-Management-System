using System.ComponentModel.DataAnnotations.Schema;

namespace OA.Infrastructure.EF.Entities
{
    [Table("InsuranceType")]

    public class InsuranceType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public virtual ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();

    }
}
