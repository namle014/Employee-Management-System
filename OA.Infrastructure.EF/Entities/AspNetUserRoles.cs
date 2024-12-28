namespace OA.Infrastructure.EF.Entities
{
    public class AspNetUserRoles
    {
        public int Id { get; set; }
        public string RoleId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
       
    }
}
