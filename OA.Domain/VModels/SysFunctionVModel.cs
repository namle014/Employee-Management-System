using OA.Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace OA.Domain.VModels
{
    public class SysFunctionCreateVModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public int? Sort { get; set; }
        public string? PathTo { get; set; }
        public string? PathIcon { get; set; }
        public bool IsActive { get; set; }
        public string? NameController { get; set; } = string.Empty;
    }

    public class SysFunctionUpdateVModel : SysFunctionCreateVModel
    {
        public int Id { get; set; }
    }

    public class SysFunctionGetByIdVModel : SysFunctionUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class SysFunctionGetAllVModel : SysFunctionUpdateVModel
    {
        public SysFunctionIsAllowVModel Function { get; set; } = new SysFunctionIsAllowVModel();
    }

    public class SysFunctionIsAllowVModel
    {
        public bool IsAllowAll { get; set; } = false;
        public bool IsAllowView { get; set; } = false;
        public bool IsAllowCreate { get; set; } = false;
        public bool IsAllowEdit { get; set; } = false;
        public bool IsAllowPrint { get; set; } = false;
        public bool IsAllowDelete { get; set; } = false;
    }

    public class UpadateJsonAPIFunctionIdVModel
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string JsonAPIFunction { get; set; } = string.Empty;
    }

    public class SysFunctionGetAllAsTreesVModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public int? Sort { get; set; }
        public string PathTo { get; set; } = string.Empty;
        public string PathIcon { get; set; } = string.Empty;
        public SysFunctionIsAllowVModel Function { get; set; } = new SysFunctionIsAllowVModel();
        public List<SysFunctionGetAllAsTreesVModel> Children { get; set; } = new List<SysFunctionGetAllAsTreesVModel>();
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }

    public class FilterSysFunctionVModel
    {
        public bool? IsActive { get; set; }
        public string? Keyword { get; set; }
        public DateTime? CreatedDate { get; set; }
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = CommonConstants.ConfigNumber.pageSizeDefault;
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = false;
    }

    public class SysFunctionExportVModel
    {

    }
}
