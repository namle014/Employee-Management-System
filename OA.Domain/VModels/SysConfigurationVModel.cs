using OA.Core.Constants;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
namespace OA.Domain.VModels
{
    public class SysConfigurationCreateVModel
    {
        [Required]
        public string Value { get; set; } = string.Empty;
        [Required]
        [MaxLength(250)]
        public string Key { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string Type { get; set; } = string.Empty;
    }

    public class SysConfigurationUpdateVModel : SysConfigurationCreateVModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Id { get; set; }
    }

    public class SysConfigurationGetByIdVModel : SysConfigurationUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class SysConfigurationGetAllVModel : SysConfigurationGetByIdVModel
    {

    }

    public class SysConfigurationChangeStatusManyVModel
    {
        public List<int> Ids { get; set; } = new List<int>();
    }

    public class FilterSysConfigurationVModel
    {
        public bool IsActive { get; set; } = true;
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

    [DataContract]
    public class SysConfigurationExportVModel
    {
        [DataMember(Name = @"Key")]
        public string Key { get; set; } = string.Empty;

        [DataMember(Name = @"Value")]
        public string Value { get; set; } = string.Empty;

        [DataMember(Name = @"Type")]
        public string Type { get; set; } = string.Empty;

        [DataMember(Name = @"Description")]
        public string Description { get; set; } = string.Empty;

        [DataMember(Name = @"CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [DataMember(Name = @"CreatedBy")]
        public string CreatedBy { get; set; } = string.Empty;
    }
}
