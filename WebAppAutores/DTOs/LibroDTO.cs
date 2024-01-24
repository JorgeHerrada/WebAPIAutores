using System.ComponentModel.DataAnnotations;
using WebAppAutores.Validations;

namespace WebAppAutores.DTOs
{
    public class LibroDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        //public List<ComentarioDTO> Comentarios { get; set; } // used for JOIN but not necessary
    }
}
