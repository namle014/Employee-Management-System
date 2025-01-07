using Microsoft.AspNetCore.Identity;

namespace OA.Infrastructure.EF.Entities
{
    public partial class AspNetUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public int? AvatarFileId { get; set; }
        public bool? Gender { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateTime? Birthday { get; set; }
        public string JsonUserHasFunctions { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public string CitizenNumber { get; set; } = string.Empty;
        public DateTime StartDateWork { get; set; }
        public bool WorkStatus { get; set; }
        public int? DepartmentId { get; set; }
        public int EmployeeDependents { get; set; }
        public string? Note { get; set; }
        public string? EmployeeId { get; set; }
        public int RemainingLeaveDays { get; set; }

        //[ForeignKey("AvatarFileId")]
        //public virtual SysFile? AvatarFile { get; set; }
    }
}
