using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.VModels
{
    public class TransferHistoryCreateVModel
    {
        public string EmployeeId { get; set; } = string.Empty;
        public int ToDepartmentId { get; set; }
        public string? Reason { get; set; }
    }
    public class TransferHistoryGetAllVModel:TransferHistoryCreateVModel {
        public int Id { get; set; }
        public DateTime TransferDate { get; set; }
        public int FromDepartmentId { get; set; }
    }
    public class TransferHistoryExportVModel { }
    public class TransferHistoryUpdateVModel { }
    public class TransferHistoryGetByIdVModel { }
}
