using OA.Core.Constants;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;


namespace OA.Domain.VModels
{
    public class SysApiCreateVModel
    {
        public string ControllerName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
    public class SysApiUpdateVModel : SysApiCreateVModel
    {
        public int Id { get; set; }
    }
    public class SysApiGetAllVModel : SysApiUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
    public class SysApiGetByIdVModel : SysApiUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }

    [DataContract]
    public class SysApiExportVModel
    {
        [DataMember(Name = @"ControllerName")]
        public string ControllerName { get; set; } = string.Empty;
        [DataMember(Name = @"ActionName")]
        public string ActionName { get; set; } = string.Empty;
        [DataMember(Name = @"HttpMethod")]
        public string HttpMethod { get; set; } = string.Empty;

        [DataMember(Name = @"CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [DataMember(Name = @"CreatedBy")]
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class FilterSysAPIVModel
    {
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = CommonConstants.ConfigNumber.pageSizeDefault;
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
        public string? SortBy { get; set; }
        public bool IsExport { get; set; } = false;
        public bool IsDescending { get; set; } = true;
        public string? Keyword { get; set; }
    }
}
