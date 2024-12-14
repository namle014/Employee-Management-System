using Newtonsoft.Json;
using OA.Core.Constants;
using OA.Infrastructure.EF.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OA.Domain.VModels.Role
{
    public class UpdateRoleModel
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> AssignRoles { get; set; } = new List<string>();
    }
    public class AspNetRoleCreateVModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
    public class AspNetRoleUpdateVModel : AspNetRoleCreateVModel
    {
        public string Id { get; set; } = string.Empty;
        public AspNetRole ToEntity(AspNetRole actionName)
        {
            actionName.Name = Name;
            actionName.IsActive = IsActive;
            return actionName;
        }
    }
    public class AspNetRoleGetAllVModel : AspNetRoleUpdateVModel
    {
        public string ConcurrencyStamp { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
    public class AspNetRoleGetByIdVModel : AspNetRoleGetAllVModel
    {
        public string JsonRoleHasFunctions { get; set; } = string.Empty;
    }
    public class UpadateJsonHasFunctionByRoleIdVModel
    {
        public string Id { get; set; } = string.Empty;
        public string JsonRoleHasFunctions { get; set; } = string.Empty;
    }
    public class RoleVModel
    {
        public string ControllerName { get; set; } = string.Empty;
        public List<RoleModel>? PermissionModels { get; set; }
        public bool IsCheckedCtrl;
    }
    public class RoleModel
    {
        public bool IsAllow { get; set; } = false;
        public string ActionName { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = string.Empty;
    }

    public class FiltersGetAllByQueryStringRoleVModel
    {
        public string Keyword { get; set; } = string.Empty;
        public string CreatedDate { get; set; } = string.Empty;
        public bool Status { get; set; }
        public string OrderBy { get; set; } = string.Empty;
        public string OrderDirection { get; set; } = string.Empty;
        public int PageSize { get; set; } = CommonConstants.ConfigNumber.pageSizeDefault;
        public int PageNumber { get; set; } = 1;
        public bool IsExport { get; set; } = false;
    }
    public class AspNetRoleExport
    {
        [DataMember(Name = @"Id")]
        public string Id { get; set; } = string.Empty;
        [DataMember(Name = @"Name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = @"CreatedDate")]
        public DateTime? CreatedDate { get; set; } = null;
        [DataMember(Name = @"CreatedBy")]
        public string? CreatedBy { get; set; }

        [DataMember(Name = @"IsActive")]
        public int? IsActive { get; set; }
    }
}
