using WebAppAutores.Validations;

namespace WebAppAutores.Controllers.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        [CustomCapitalized]
        public string Titulo { get; set; }
    }
}
