using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Entities
{
    public partial class AspNetUser : IdentityUser
    {
        public string? EmployeeId { get; set; }
        public string? Name { get; set; }
        public long? AvatarFileId { get; set; }
        public bool? Gender { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? JsonUserHasFunctions { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? IsActive { get; set; }
        public string? CitizenNumber { get; set; }
        public DateTime? CitizenNumberDate { get; set; }
        public string? IsIssuedBy { get; set; }

        //[ForeignKey("AvatarFileId")]
        //public virtual SysFile? AvatarFile { get; set; }
    }
}
