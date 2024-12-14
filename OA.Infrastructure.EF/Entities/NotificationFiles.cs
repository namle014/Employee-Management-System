namespace OA.Infrastructure.EF.Entities
{
    public class NotificationFiles
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public int FileId { get; set; }
    }
}
