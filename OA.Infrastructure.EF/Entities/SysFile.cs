namespace OA.Infrastructure.EF.Entities
{
    public partial class SysFile : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
