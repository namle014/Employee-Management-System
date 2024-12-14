using Microsoft.AspNetCore.Http;
using OA.Core.Constants;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OA.Domain.VModels
{
    public class SysFileCreateVModel
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string UniqueFileName { get; set; } = string.Empty;
    }

    public class SysFileCreateBase64VModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Base64String { get; set; } = string.Empty;
    }

    public class FileChunk
    {
        public string FileName { get; set; } = string.Empty;
        public IFormFile File { get; set; } = default!;
        public int ChunkIndex { get; set; }
        public int TotalChunks { get; set; }
        public string UniqueFileName { get; set; } = string.Empty;
    }

    public class SysFileBase64ToFileVModel
    {
        public Guid SessionId { get; set; }
        public int PartNumber { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Base64 { get; set; } = string.Empty;
        public bool IsEnd { get; set; }
    }
    public class SysFileUpdateVModel
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Id { get; set; }
    }
    public class SysFileGetAllVModel : SysFileUpdateVModel
    {
        public string Path { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class SysFileGetByIdVModel : SysFileGetAllVModel
    {

    }

    public class FilterSysFileVModel
    {
        public bool? IsActive { get; set; }
        public string? Keyword { get; set; }
        public DateTime? CreatedDate { get; set; }
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
        [DataMember(Name = @"IsActive")]
        public bool? IsActive { get; set; }
    }
}
