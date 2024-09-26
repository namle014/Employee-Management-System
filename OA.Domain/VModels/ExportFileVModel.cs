using OA.Core.CustomValidationAttribute;
using System.ComponentModel.DataAnnotations;

namespace OA.Core.VModels
{
    public class ExportFileVModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$")]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$")]
        public string SheetName { get; set; } = string.Empty;

        [Required]
        [StringInList("EXCEL", "PDF")]
        public string Type { get; set; } = string.Empty;
    }
}
