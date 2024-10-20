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
        public int Id { get; set; }
        //public string? CreatedBy { get; set; }
        //public DateTime? CreatedDate { get; set; }
        //public string? UpdatedBy { get; set; }
        //public DateTime? UpdatedDate { get; set; }
        public string Name { get; set; } = string.Empty;
        public int InsuranceTypeId { get; set; }
        public double InsuranceContribution { get; set; }
        public bool IsActive { get; set; }

    }

    public class InsuranceUpdateVModel : InsuranceCreateVModel
    {
    }

    public class InsuranceGetByIdVModel : InsuranceCreateVModel
    {
    }

    public class InsuranceGetAllVModel : InsuranceCreateVModel
    {

    }

    public class FilterInsuranceVModel
    {
      
    }

    [DataContract]
    public class InsuranceExportVModel
    {

    }
}
