﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Entities
{
    public class NotificationRoles
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public string RoleId { get; set; } = string.Empty;
    }
}
