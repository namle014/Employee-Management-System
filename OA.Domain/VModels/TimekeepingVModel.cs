using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.VModels
{
    public class TimekeepingCreateVModel
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }
        public string CheckInIP { get; set; } = string.Empty;
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
