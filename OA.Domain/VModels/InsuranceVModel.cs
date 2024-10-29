using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.VModels
{
    public class InsuranceCreateVModel
    {
        public string? CreatedBy { get; set; }
        public string Name { get; set; } = string.Empty;
        public int InsuranceTypeId { get; set; }
        public decimal InsuranceContribution { get; set; }

    }

    public class InsuranceUpdateVModel : InsuranceCreateVModel
    {
        public string Id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? UpdatedBy { get; set; }

    }

    public class InsuranceGetByIdVModel : InsuranceCreateVModel
    {
        public string FullName { get; set; } = string.Empty;

    }

    public class InsuranceGetAllVModel : InsuranceCreateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class FilterInsuranceVModel
    {
      
    }

    [DataContract]
    public class InsuranceExportVModel
    {

    }
}
