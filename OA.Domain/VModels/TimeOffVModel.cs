using OA.Core.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;


namespace OA.Domain.VModels
{
    public class TimeOffCreateVModel
    {
        public string? Reason { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }
    }

    public class TimeOffUpdateVModel : TimeOffCreateVModel
    {
        public int Id { get; set; }
    }
    public class TimeOffGetAllVModel : TimeOffUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
    public class TimeOffGetByIdVModel : TimeOffUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }

    [DataContract]
    public class TimeOffExportVModel
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

    public class FilterTimeOffVModel
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
