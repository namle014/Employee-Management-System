namespace OA.Infrastructure.EF.Entities
{
    public partial class SysApi : BaseEntity
    {
        public string ControllerName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = string.Empty;
    }
}
