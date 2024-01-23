using System.ComponentModel.DataAnnotations;
using WebAppAutores.Validations;

namespace WebAppAutores.Controllers.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        [CustomCapitalized]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }
    }
}
