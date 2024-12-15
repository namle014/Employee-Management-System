namespace OA.Infrastructure.EF.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? DepartmentHeadId { get; set; }

    }
}
