using OA.Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace OA.Core.VModels
{
    public class HolidayCreateVModel
    {
        public string Name { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Note { get; set; } = string.Empty;
    }
    public class HolidayUpdateVModel : HolidayCreateVModel 
    { 
        public int Id { get; set; }
    }
    public class HolidayGetAllVModel : HolidayUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
    public class HolidayGetByIdVModel : HolidayUpdateVModel { }
    public class HolidayExportVModel
    {

    }
    public class HolidayFilterVModel
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
    public class HolidayVModel
    {
    }
    public class HolidayDeleteManyVModel
    {
        public List<int> Ids { get; set; } = new List<int>();
    }
}
