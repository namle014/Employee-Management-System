using OA.Core.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;


namespace OA.Domain.VModels
{
    public class EmploymentContractCreateVModel
    {
        public string UserId { get; set; } = string.Empty;
        public string ContractName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal BasicSalary { get; set; }
        public string? Clause { get; set; }
        public bool IsActive { get; set; }
    }

    public class EmploymentContractUpdateVModel : EmploymentContractCreateVModel
    {
        public int Id { get; set; }
    }
    public class EmploymentContractGetAllVModel : EmploymentContractUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
    public class EmploymentContractGetByIdVModel : EmploymentContractUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }

    [DataContract]
    public class EmploymentContractExportVModel
    {
        [DataMember(Name = @"UserId")]
        public string UserId { get; set; } = string.Empty;

        [DataMember(Name = @"ContractName")]
        public string ContractName { get; set; } = string.Empty;

        [DataMember(Name = @"StartDate")]
        public DateTime StartDate { get; set; }

        [DataMember(Name = @"EndDate")]
        public DateTime EndDate { get; set; }

        [DataMember(Name = @"BasicSalary")]
        public decimal BasicSalary { get; set; }

        [DataMember(Name = @"Clause")]
        public string Clause { get; set; } = string.Empty;

        [DataMember(Name = @"CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [DataMember(Name = @"CreatedBy")]
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class FilterEmploymentContractVModel
    {
        public bool? IsActive { get; set; }
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
   
}
