using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAppAutores.Validations;

namespace WebAppAutores.Controllers.Entidades
{
    public class Autor: IValidatableObject  // IValidatableObject is necessary for Model Validation
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo '{0}' es requerido")] // Mandatory param and errorMessage
        [StringLength(maximumLength: 10, ErrorMessage = "El campo {0} no debe de tener más de {1} caracteres")]
        // [CustomCapitalized] // custom validation
        public string Nombre { get; set; }
        [Range(18,120)]
        [NotMapped] // no se agrega a la base de datos 
        public int Edad { get; set; }
        [CreditCard] // solo valida numeración
        [NotMapped]
        public string TarjetaDeCredito { get; set; }
        [Url]
        [NotMapped]
        public string URL { get; set; }
        public List<Libro> Libros { get; set; }
        public int Menor { get; set; }
        public int Mayor { get; set; }

        // validation at the Model level
        // it returns a list of validations
        // MODEL validations are executed once ALL ATTRIBUTE validations are successfuly completed
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validate capitalization
            if (!string.IsNullOrEmpty(Nombre))
            {
                var firstChar = Nombre[0].ToString();

                if (firstChar != firstChar.ToUpper())
                {
                    // yield add the validation to the list to return it later
                    yield return new ValidationResult("The field must be capitalized (Model)",
                        new string[] { nameof(Nombre)});
                }
            }

            // Validate that Menor < Mayor
           if (Menor > Mayor)
            {
                yield return new ValidationResult("Menor must be equal or lower than Mayor", 
                    new string[] {nameof(Menor)}); // defines under what field the error will be shown
            }
        }
    }
}
