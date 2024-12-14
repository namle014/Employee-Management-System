namespace OA.Infrastructure.EF.Entities
{
    public class Holiday : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
