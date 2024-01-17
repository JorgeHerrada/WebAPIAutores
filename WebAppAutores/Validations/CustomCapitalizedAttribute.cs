using System.ComponentModel.DataAnnotations;

namespace WebAppAutores.Validations
{
    public class CustomCapitalizedAttribute: ValidationAttribute
    {
        // overides IsValid method and create custom one 
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var firstChar = value.ToString()[0].ToString();

            if (firstChar != firstChar.ToUpper())
            {
                return new ValidationResult("The field must be Capitalized");
            }

            return ValidationResult.Success;
        }
    }
}
