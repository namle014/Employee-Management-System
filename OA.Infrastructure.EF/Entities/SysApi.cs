using System;

namespace OA.Infrastructure.EF.Entities
{
    public partial class SysApi : BaseEntity
    {
        public string ControllerName { get; set; } = null!;
        public string ActionName { get; set; } = null!;
        public string HttpMethod { get; set; } = string.Empty;
    }
}
