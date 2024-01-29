using System.ComponentModel.DataAnnotations;
using WebAppAutores.Validations;

namespace WebAppAutores.DTOs
{
    public class LibroPatchDTO
    {
        [CustomCapitalized]
        [StringLength(maximumLength: 250)]
        [Required]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
