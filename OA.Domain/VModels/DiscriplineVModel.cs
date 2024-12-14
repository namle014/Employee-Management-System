using OA.Core.Constants;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OA.Core.VModels
{
    public class DisciplineCreateVModel
    {
        public string UserId { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public decimal Money { get; set; }
        public string? Note { get; set; }
        public bool IsActive { get; set; }

    }

    public class DisciplineUpdateVModel : DisciplineCreateVModel
    {
        public int Id { get; set; }
    }

    public class DisciplineGetAllVModel : DisciplineUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class DisciplineGetByIdVModel : DisciplineUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
    [DataContract]
    public class DisciplineExportVModel
    {
        [DataMember(Name = @"UserId")]
        public string UserId { get; set; } = string.Empty;

        [DataMember(Name = @"Reason")]
        public string? Reason { get; set; } 

        [DataMember(Name = @"Money")]
        public double Money { get; set; }

        [DataMember(Name = @"Note")]
        public string? Note { get; set; }


        [DataMember(Name = @"CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [DataMember(Name = @"CreatedBy")]
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class DisciplineFilterVModel
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
