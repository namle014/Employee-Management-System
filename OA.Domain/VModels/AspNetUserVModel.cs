using OA.Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace OA.Domain.VModels
{
    public class UpdatePermissionVModel
    {
        public string UserId { get; set; } = string.Empty;
        public string JsonUserHasFunctions { get; set; } = string.Empty;
    }
    public class CredentialsVModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RequestResetPassword
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordModel
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UserFilterVModel
    {
        public string? Keyword { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? FullName { get; set; } = string.Empty;
        public DateTime? Birthday { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = true;
        public int? AvatarFileId { get; set; }
        public int PageSize { get; set; } = CommonConstants.ConfigNumber.pageSizeDefault;
        public int PageNumber { get; set; } = 1;
    }

    public class UserChangePasswordVModel
    {
        [Required]
        public string OldPassword { get; set; } = string.Empty;
        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
    public class UserVModel
    {
        public string? FullName { get; set; } = string.Empty;
        [Required]
        [RegularExpression("^[a-zA-Z0-9]*$")]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [RegularExpression("^[0-9]*$")]
        public string? PhoneNumber { get; set; }
        public DateTime StartDateWork { get; set; }
        public int? AvatarFileId { get; set; }
        public bool? Gender { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime? Birthday { get; set; }
        public int DepartmentId { get; set; }
        public int EmployeeDependents { get; set; }
        public string? EmployeeId { get; set; }
    }

    public class UserCreateVModel : UserVModel
    {
        [Required]
        public string Password { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public bool? IsActive { get; set; }
    }

    public class UserUpdateVModel : UserVModel
    {
        public string Id { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class UserGetAllVModel : UserUpdateVModel
    {
        public string DepartmentName { get; set; } = string.Empty;
        public string? AvatarPath { get; set; }
    }

    public class GetMeVModel : UserGetAllVModel
    {
        public bool IsAdmin { get; set; } = false;
        public int RemainingLeaveDays { get; set; }
        public List<MenuLeft> MenuLeft { get; set; } = new List<MenuLeft>();
    }

    public class MenuLeft
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PathTo { get; set; } = string.Empty;
        public int Sort { get; set; }
        public string PathIcon { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public string? NameController { get; set; } = string.Empty;
        public Function Function { get; set; } = new Function();
        public List<MenuLeft> Childs { get; set; } = new List<MenuLeft>();
    }

    public class Function
    {
        public bool IsAllowAll { get; set; }
        public bool IsAllowView { get; set; }
        public bool IsAllowEdit { get; set; }
        public bool IsAllowCreate { get; set; }
        public bool IsAllowPrint { get; set; }
        public bool IsAllowDelete { get; set; }
    }
    public class UserGetByIdVModel : UserGetAllVModel
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
    }

    public class ConfirmAccount
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string Code { get; set; } = string.Empty;
    }
}
