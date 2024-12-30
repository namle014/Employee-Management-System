using System.Runtime.Serialization;

namespace OA.Core.VModels
{
    public class TimekeepingCreateVModel
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }
        public string? CheckInIP { get; set; }
        public string? UserAgent { get; set; }
    }

    public class TimekeepingUpdateVModel : TimekeepingCreateVModel
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }

    public class TimekeepingGetByIdVModel : TimekeepingUpdateVModel
    {
        public string FullName { get; set; } = string.Empty;
    }

    public class TimekeepingGetAllVModel : TimekeepingGetByIdVModel
    {

    }

    public class FilterTimekeepingVModel
    {
        public string? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }

    [DataContract]
    public class TimekeepingExportVModel
    {

    }
}
