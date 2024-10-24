using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class HolidayVModel
    {
    }
}
