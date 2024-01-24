using System.ComponentModel.DataAnnotations;
using WebAppAutores.Validations;

namespace WebAppAutores.Controllers.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        [Required]
        [CustomCapitalized]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }
        public List<Comentario> Comentarios { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; } // access Libro's Autores
    }
}
