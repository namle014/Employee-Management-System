﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Entities
{
    public partial class SysFunction : BaseEntity
    {
        public string Name { get; set; } = null!;
        public long? ParentId { get; set; }
        public int? Sort { get; set; }
        public string? PathTo { get; set; }
        public string? PathIcon { get; set; }
    }
}
