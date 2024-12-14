using System.Runtime.Serialization;

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

    public class InsuranceGetByIdVModel : InsuranceUpdateVModel
    {
        public string NameOfInsuranceType { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }

    public class InsuranceGetAllVModel : InsuranceGetByIdVModel
    {

    }

    public class FilterInsuranceVModel
    {
        public string Id { get; set; } = string.Empty;
        public string NameofInsuranceType { get; set; } = string.Empty;
        public decimal InsuranceContribution { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
        public string Keyword { get; set; } = string.Empty;
    }

    [DataContract]
    public class InsuranceExportVModel
    {

    }
}
