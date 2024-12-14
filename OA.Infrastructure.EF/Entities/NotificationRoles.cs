namespace OA.Infrastructure.EF.Entities
{
    public class NotificationRoles
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public string RoleId { get; set; } = string.Empty;
    }
}
