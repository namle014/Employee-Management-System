namespace OA.Infrastructure.EF.Entities
{
    public class Notifications
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentTime { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Url { get; set; }
        public string? Type { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int TypeToNotify { get; set; }
    }
}
