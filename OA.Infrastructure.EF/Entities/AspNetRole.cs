using Microsoft.AspNetCore.Identity;
using System;
namespace OA.Infrastructure.EF.Entities
{
    public class AspNetRole : IdentityRole
    {
        public string JsonRoleHasFunctions { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
}
