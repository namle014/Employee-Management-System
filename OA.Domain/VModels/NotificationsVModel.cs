using System.Text.Json.Serialization;

namespace OA.Core.VModels
{
    public class NotificationsCreateVModel
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string>? ListUser { get; set; }
        public List<int>? ListFile { get; set; }
        public List<string>? ListRole { get; set; }
        public List<int>? ListDept { get; set; }
        public string Type { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int TypeToNotify { get; set; }
    }

    public class NotificationsUpdateVModel : NotificationsCreateVModel
    {
        public int Id { get; set; }
    }

    public class UserNotificationsUpdateIsNewVModel
    {
        public string UserId { get; set; } = string.Empty;
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
        public string? Type { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? FullName { get; set; } = string.Empty;
        public string? AvatarPath { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int? ReceivedCount { get; set; }
        public int? ReadCount { get; set; }
    }

    public class NotificationsGetByIdVModel : NotificationsGetAllVModel
    {
        public string? Role { get; set; } = string.Empty;
        public List<string>? ListFile { get; set; }
        public List<string>? ListUser { get; set; }
        public List<string>? ListUserRead { get; set; }
    }

    public class NotificationsGetAllForUserVModel
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }
        [JsonPropertyName("UserId")]
        public string UserId { get; set; } = string.Empty;
        [JsonPropertyName("Title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("Content")]
        public string Content { get; set; } = string.Empty;
        [JsonPropertyName("SentTime")]
        public DateTime SentTime { get; set; }
        [JsonPropertyName("Type")]
        public string? Type { get; set; }
        [JsonPropertyName("IsRead")]
        public bool IsRead { get; set; }
        [JsonPropertyName("NotificationId")]
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

    public class FilterCountNotifyReadByUser
    {
        public int PageNumber { get; set; }
        public string? FullName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class FilterNotificationsForUserVModel
    {
        public bool IsActive { get; set; } = true;
        public bool? IsRead { get; set; }
        public DateTime? SentDate { get; set; }
    }
}
