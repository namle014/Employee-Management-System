using System.ComponentModel.DataAnnotations;

namespace OA.Core.CustomValidationAttribute
{
    public class StringInListAttribute : ValidationAttribute
    {
        private readonly List<string> _validValues;

        public StringInListAttribute(params string[] validValues)
        {
            _validValues = validValues.ToList();
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || !_validValues.Contains(value.ToString() ?? string.Empty))
            {
                string errorMessage = $"The field {validationContext.DisplayName} must be one of the following values: {string.Join(", ", _validValues)}.";
                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
