using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAppAutores.Validations;

namespace WebAppAutores.Controllers.Entidades
{
    public class Autor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo '{0}' es requerido")] // Mandatory param and errorMessage
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe de tener más de {1} caracteres")]
        [CustomCapitalized] // custom validation
        public string Nombre { get; set; }
    }
}
