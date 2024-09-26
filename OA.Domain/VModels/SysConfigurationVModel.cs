using OA.Core.CustomValidationAttribute;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
namespace OA.Domain.VModels
{
    public class SysConfigurationCreateVModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]*$")]
        public string Value { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]*$")]
        [MaxLength(250)]
        public string Key { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
    public class SysConfigurationUpdateVModel : SysConfigurationCreateVModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Id { get; set; }
    }
    public class SysConfigurationGetByIdVModel : SysConfigurationUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
    public class SysConfigurationGetAllVModel : SysConfigurationGetByIdVModel
    {
    }

    [DataContract]
    public class SysConfigurationExportVModel
    {
        [DataMember(Name = @"Key")]
        public string Key { get; set; } = string.Empty;

        [DataMember(Name = @"Giá trị")]
        public string Value { get; set; } = string.Empty;

        [DataMember(Name = @"Mô tả")]
        public string Description { get; set; } = string.Empty;
    }
}
