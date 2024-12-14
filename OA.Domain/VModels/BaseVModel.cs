using OA.Core.Constants;
using System.ComponentModel.DataAnnotations;
namespace OA.Core.VModels
{
    public class FilterGetAllVModel
    {
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = CommonConstants.ConfigNumber.pageSizeDefault;
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
        public string? SortBy { get; set; }
        public bool IsExport { get; set; } = false;
        public bool IsDescending { get; set; } = false;
        public string? Keyword { get; set; }
    }

    public class FiltersGetAllByQueryStringVModel
    {
        public string? Keyword { get; set; }
        public string? CreatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? OrderBy { get; set; }
        public string? OrderDirection { get; set; }
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = CommonConstants.ConfigNumber.pageSizeDefault;
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
    }
}
