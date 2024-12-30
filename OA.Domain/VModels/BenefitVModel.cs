using OA.Core.Constants;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OA.Core.VModels
{
    public class BenefitCreateVModel
    {
        public string Name { get; set; } = string.Empty;
        public int BenefitTypeId { get; set; }
        //public bool IsActive { get; set; }

    }
    public class BenefitUpdateVModel : BenefitCreateVModel
    {
        public string Id { get; set; } = string.Empty;


    }

    public class BenefitGetByIdVModel : BenefitUpdateVModel
    {
        public string NameOfBenefitType { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }

    public class BenefitGetAllVModel : BenefitGetByIdVModel
    {

    }

    public class BenefitTypeGetAllVModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        //public string? Description { get; set; }
    }

    public class BenefitTypeCreateVModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description {  get; set; }
    }

    public class BenefitTypeUpdateVModel : BenefitTypeCreateVModel { 
        public int Id { get; set; }
    }  



    [DataContract]
    public class BenefitExportVModel
    {
    }

    public class FilterBenefitVModel
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

    public class BenefitChangeStatusManyVModel
    {
        public List<string> Ids { get; set; } = new List<string>();
    }

    public class CreateBenefitUser
    {
        public string? UserId { get; set; }
        public string? BenefitId { get; set; }
        public decimal BenefitContribution { get; set; }
    }

    public class GetAllBenefitUser
    {
        public int PageSize { get; set; } = CommonConstants.ConfigNumber.pageSizeDefault;
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
        public string? SortBy { get; set; }
        public bool IsExport { get; set; } = false;
        public bool IsDescending { get; set; } = true;
        public string? Keyword { get; set; }
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool? Gender { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string? DepartmentName{ get; set; }
        public List<int> DepartmentIds { get; set; } = new List<int>();
        public decimal FromBenefitContribution { get; set; }
        public decimal ToBenefitContribution { get; set; }
        public string BenefitName { get; set; } = string.Empty;
        public decimal BenefitContribution { get; set; }

    }

}
