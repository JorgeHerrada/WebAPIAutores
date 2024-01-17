using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAppAutores.Validations;

namespace WebAppAutores.Controllers.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo '{0}' es requerido")] // Mandatory param and errorMessage
        [StringLength(maximumLength: 10, ErrorMessage = "El campo {0} no debe de tener más de {1} caracteres")]
        [CustomCapitalized] // custom validation
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
    }
}
