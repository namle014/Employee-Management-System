using Microsoft.AspNetCore.Identity;
namespace OA.Infrastructure.EF.Entities
{
    public class AspNetRole : IdentityRole
    {
        public string? JsonRoleHasFunctions { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? IsActive { get; set; }
        //public string ?Description { get; set; }
    }
}
