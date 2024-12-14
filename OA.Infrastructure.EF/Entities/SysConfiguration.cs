namespace OA.Infrastructure.EF.Entities
{
    public partial class SysConfiguration : BaseEntity
    {
        public string Value { get; set; } = null!;
        public string Key { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Type { get; set; } = string.Empty;
    }
}
