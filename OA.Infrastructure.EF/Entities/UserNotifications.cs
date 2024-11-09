using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Entities
{
    public class UserNotifications
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int NotificationId { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}
