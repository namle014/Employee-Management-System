using OA.Core.CustomValidationAttribute;
using System.ComponentModel.DataAnnotations;

namespace OA.Core.VModels
{
    public class ExportFileVModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$")]
        public string? FileName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$")]
        public string? SheetName { get; set; }

        [Required]
        [StringInList("EXCEL", "PDF")]
        public string? Type { get; set; }
    }
}
