using OA.Core.Constants;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OA.Core.VModels
{
    public class DepartmentCreateVModel
    {
        public string Name { get; set; } = string.Empty;
        public string? DepartmentHeadId { get; set; }

    }

    public class DepartmentUpdateVModel : DepartmentCreateVModel
    {
        public int Id { get; set; }
    }

    public class DepartmentGetAllVModel : DepartmentUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public int CountDepartment { get; set; } = 0;
        public string? DepartmentHeadName { get; set; }
    }

    public class DepartmentGetByIdVModel : DepartmentUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
    [DataContract]
    public class DepartmentExportVModel
    {
        [DataMember(Name = @"Name")]
        public string Name { get; set; } = string.Empty;
        [DataMember(Name = @"DepartmentHeadId")]
        public string? DepartmentHeadId { get; set; }

        [DataMember(Name = @"CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [DataMember(Name = @"CreatedBy")]
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class DepartmentFilterVModel
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

    public class DepartmentChangeStatusManyVModel
    {
        public List<int> Ids { get; set; } = new List<int>();
    }
}
