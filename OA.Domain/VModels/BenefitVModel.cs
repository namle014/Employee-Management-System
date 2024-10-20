using OA.Core.Constants;
using OA.Domain.VModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.VModels
{
    public class BenefitVModel
    {
        public int Id { get; set; }
        //public string? CreatedBy { get; set; }
        //public DateTime? CreatedDate { get; set; }
        //public string? UpdatedBy { get; set; }
        //public DateTime? UpdatedDate { get; set; }
        public string Name { get; set; } = string.Empty;
        public int BenefitTypeId { get; set; }
        public double BenefitContribution { get; set; }
        public bool IsActive { get; set; }
    }
    public class BenefitUpdateVModel : BenefitVModel
    {
    }

    public class BenefitGetAllVModel : BenefitVModel
    {
        //public DateTime? CreatedDate { get; set; }
        //public string? CreatedBy { get; set; } = string.Empty;
        //public DateTime? UpdatedDate { get; set; }
        //public string? UpdatedBy { get; set; }
    }
    public class BenefitGetByIdVModel : BenefitVModel
    {
        //public DateTime? CreatedDate { get; set; }
        //public string? CreatedBy { get; set; }
        //public DateTime? UpdatedDate { get; set; }
        //public string? UpdatedBy { get; set; }
    }

    [DataContract]
    public class BenefitExportVModel
    {
        //    [DataMember(Name = @"ControllerName")]
        //    public string ControllerName { get; set; } = string.Empty;
        //    [DataMember(Name = @"ActionName")]
        //    public string ActionName { get; set; } = string.Empty;
        //    [DataMember(Name = @"HttpMethod")]
        //    public string HttpMethod { get; set; } = string.Empty;

        //    [DataMember(Name = @"CreatedDate")]
        //    public DateTime CreatedDate { get; set; }

        //    [DataMember(Name = @"CreatedBy")]
        //    public string CreatedBy { get; set; } = string.Empty;
    }

    public class FilterBenefitVModel
    {
        //public bool? IsActive { get; set; }
        //public DateTime? CreatedDate { get; set; }
        //[Range(1, int.MaxValue)]
        //public int PageSize { get; set; } = CommonConstants.ConfigNumber.pageSizeDefault;
        //[Range(1, int.MaxValue)]
        //public int PageNumber { get; set; } = 1;
        //public string? SortBy { get; set; }
        //public bool IsExport { get; set; } = false;
        //public bool IsDescending { get; set; } = true;
        //public string? Keyword { get; set; }
    }
}
