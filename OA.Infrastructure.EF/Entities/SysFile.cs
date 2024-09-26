using System;
using System.Collections.Generic;

namespace OA.Infrastructure.EF.Entities
{
    public partial class SysFile : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Path { get; set; } = null!;
        public string Type { get; set; } = string.Empty;
    }
}
