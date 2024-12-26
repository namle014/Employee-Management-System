using OA.Core.Constants;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using static OA.Core.Constants.MsgConstants;

namespace OA.Domain.VModels
{
    public class JobHistoryCreateVModel
    {
        public string EmployeeId { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Note { get; set; }
        public string? JobDescription { get; set; }
        public string? SupervisorId { get; set; }
        public string? WorkLocation { get; set; }
        public string? Allowance { get; set; }
    }

    public class JobHistoryUpdateVModel : JobHistoryCreateVModel
    {
        public int Id { get; set; }
    }

    public class FilterJobHistoryVModel
    {
        public string? Keyword { get; set; }
        public int PageSize { get; set; } = CommonConstants.ConfigNumber.pageSizeDefault;
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
        public bool IsDescending { get; set; } = true;
        public bool IsExport { get; set; } = false;
    }

    public class JobHistoryExportVModel
    {
        public string EmployeeId { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Note { get; set; }
        public string? JobDescription { get; set; }
        public string? SupervisorId { get; set; }
        public string? WorkLocation { get; set; }
        public string? Allowance { get; set; }
    }

}
