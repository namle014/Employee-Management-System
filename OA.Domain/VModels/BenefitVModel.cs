using System.Runtime.Serialization;

namespace OA.Core.VModels
{
    public class BenefitCreateVModel
    {
        public string? CreatedBy { get; set; }
        public string Name { get; set; } = string.Empty;
        public int BenefitTypeId { get; set; }
        public decimal BenefitContribution { get; set; }
    }
    public class BenefitUpdateVModel : BenefitCreateVModel
    {
        public string Id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? UpdatedBy { get; set; }


    }

    public class BenefitGetByIdVModel : BenefitUpdateVModel
    {
        public string NameOfBenefitType { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class BenefitGetAllVModel : BenefitGetByIdVModel
    {

    }


    [DataContract]
    public class BenefitExportVModel
    {
    }

    public class FilterBenefitVModel
    {
        public string Id { get; set; } = string.Empty;
        public string NameofBenefitType { get; set; } = string.Empty;
        public decimal BenefitContribution { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
        public string Keyword { get; set; } = string.Empty;
    }
}
