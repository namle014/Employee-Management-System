using OA.Core.Constants;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;


namespace OA.Domain.VModels
{
    public class WorkShiftCreateVModel
    {
        public string ShiftName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class WorkShiftUpdateVModel : WorkShiftCreateVModel
    {
        public int Id { get; set; }
    }

    public class WorkShiftGetAllVModel : WorkShiftUpdateVModel
    {
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class WorkShiftGetByIdVModel : WorkShiftGetAllVModel
    {

    }

    [DataContract]
    public class WorkShiftExportVModel
    {
        [DataMember(Name = @"ShiftName")]
        public string ShiftName { get; set; } = string.Empty;
        [DataMember(Name = @"StartTime")]
        public TimeSpan StartTime { get; set; }
        [DataMember(Name = @"EndTime")]
        public TimeSpan EndTime { get; set; }
        [DataMember(Name = @"Description")]
        public string Description { get; set; } = string.Empty;

        [DataMember(Name = @"CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [DataMember(Name = @"CreatedBy")]
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class FilterWorkShiftVModel
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
}
