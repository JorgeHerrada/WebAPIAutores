using System.ComponentModel.DataAnnotations;
using WebAppAutores.Validations;

namespace WebAppAutores.DTOs
{
    public class LibroCreationDTO
    {
        [CustomCapitalized]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }
        public List<int> AutoresIds { get; set; }
    }
}
