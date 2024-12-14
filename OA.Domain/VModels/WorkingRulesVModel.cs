using OA.Core.Constants;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OA.Core.VModels
{
    public class WorkingRulesCreateVModel
    {
        public string? Name { get; set; }
        public string? Content { get; set; }
        public string? Note { get; set; }

    }

    public class WorkingRulesUpdateVModel : WorkingRulesCreateVModel
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }

    }

    public class WorkingRulesGetAllVModel : WorkingRulesUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class WorkingRulesGetByIdVModel : RewardUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
    [DataContract]
    public class WorkingRulesExportVModel
    {
        [DataMember(Name = @"Name")]
        public string? Name { get; set; }

        [DataMember(Name = @"Content")]
        public string? Content { get; set; } 

        [DataMember(Name = @"Note")]
        public string? Note { get; set; }

        [DataMember(Name = @"CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [DataMember(Name = @"CreatedBy")]
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class WorkingRulesFilterVModel
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
