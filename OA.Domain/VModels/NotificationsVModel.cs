using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.VModels
{
    public class NotificationsCreateVModel
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string>? ListUser { get; set; }
        public List<int>? ListFile { get; set; }
        public DateTime SentTime { get; set; }
        public string Type { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }

    public class NotificationsUpdateVModel : NotificationsCreateVModel
    {
        public int Id { get; set; }
    }

    public class NotificationsUpdateReadVModel
    {
        public int Id { get; set; }
    }

    public class NotificationsUpdateAllReadVModel
    {
        public string UserId { get; set; } = string.Empty;
    }

    public class NotificationsGetAllVModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentTime { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class NotificationsGetByIdVModel : NotificationsGetAllVModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarPath { get; set; } = string.Empty;
        public string? Role { get; set; } = string.Empty;
        public List<string>? ListFile { get; set; }
        public List<string>? ListUser { get; set; }
        public List<string>? ListUserRead { get; set; }
    }

    public class NotificationsGetAllForUserVModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentTime { get; set; }
        public string? Type { get; set; }
        public bool IsRead { get; set; }
        public int NotificationId { get; set; }
    }

    public class FilterNotificationsVModel
    {
        public string? Type { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string? Title { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? SentDate { get; set; }
    }

    public class FilterNotificationsForUserVModel
    {
        public string UserId { get; set; } = string.Empty;
        public string? Type { get; set; }
        public int PageSize { get; set; } = 30;
        public int PageNumber { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public bool? IsRead { get; set; }
        public DateTime? SentDate { get; set; }
    }
}
