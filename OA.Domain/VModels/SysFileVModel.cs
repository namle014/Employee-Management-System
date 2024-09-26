using Microsoft.AspNetCore.Http;
using OA.Core.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace OA.Domain.VModels
{
    public class SysFileCreateVModel
    {
        public string Name { get; set; } = null!;
        public string Path { get; set; } = null!;
        public string Type { get; set; } = string.Empty;
    }
    public class SysFileCreateBase64VModel : SysFileCreateVModel
    {
        [Required]
        public string Base64String { get; set; } = string.Empty;
    }
    public class FileChunk
    {
        public string FileName { get; set; } = string.Empty;
        public IFormFile File { get; set; } = default!;
    }
    public class SysFileBase64ToFileVModel
    {
        public Guid SessionId { get; set; }
        public int PartNumber { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Base64 { get; set; } = string.Empty;
        public bool IsEnd { get; set; }
    }
    public class SysFileUpdateVModel : SysFileCreateVModel
    {
        public int Id { get; set; }
    }
    public class SysFileGetAllVModel : SysFileUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class SysFileGetByIdVModel : SysFileGetAllVModel
    {

    }

    public class FilterSysFileVModel
    {
        public bool? IsActive { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public string? Type { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = CommonConstants.ConfigNumber.pageSizeDefault;
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
        public string? SortBy { get; set; }
        public bool IsExport { get; set; } = false;
        public bool IsDescending { get; set; } = false;
    }

    [DataContract]
    public class SysFileExportVModel
    {
        [DataMember(Name = @"Id")]
        public long Id { get; set; }
        [DataMember(Name = @"Name")]
        public string Name { get; set; } = null!;
        [DataMember(Name = @"Path")]
        public string Path { get; set; } = null!;
        [DataMember(Name = @"Type")]
        public string Type { get; set; } = string.Empty;
        [DataMember(Name = @"CreatedDate")]
        public DateTime? CreatedDate { get; set; }
        [DataMember(Name = @"CreatedBy")]
        public string CreatedBy { get; set; } = string.Empty;
        [DataMember(Name = @"UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
        [DataMember(Name = @"UpdatedBy")]
        public string UpdatedBy { get; set; } = string.Empty;
        [DataMember(Name = @"IsActive")]
        public bool? IsActive { get; set; }
    }
}
