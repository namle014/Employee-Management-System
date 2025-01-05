using OA.Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace OA.Core.VModels
{
    public class EventCreateVModel
    {
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsHoliday { get; set; } = false;
        public string Description { get; set; } = string.Empty;
        public string? Color { get; set; }
        public bool AllDay { get; set; }
    }

    public class EventUpdateVModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Color { get; set; }
        public bool AllDay { get; set; }
    }

    public class EventGetAllVModel : EventUpdateVModel
    {
        public bool IsHoliday { get; set; } = false;
    }

    public class EventGetByIdVModel : EventUpdateVModel
    {

    }

    public class EventExportVModel
    {

    }

    public class EventFilterVModel
    {
        public bool? IsHoliday { get; set; }
        public bool IsActive { get; set; } = true;
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = CommonConstants.ConfigNumber.pageSizeDefault;
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
        public string? SortBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsExport { get; set; } = false;
        public bool IsDescending { get; set; } = true;
        public string? Keyword { get; set; }
    }

    public class EventDeleteManyVModel
    {
        public List<int> Ids { get; set; } = new List<int>();
    }
}
