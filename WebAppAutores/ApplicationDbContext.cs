using Microsoft.EntityFrameworkCore;
using WebAppAutores.Controllers.Entidades;

namespace WebAppAutores
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        // creacion de tabla Autores con las propiedades de la clase Autor 
        public DbSet<Autor> Autores { get; set; }

        // creacion de tabla Libros con propiedades de Libro
        public DbSet<Libro> Libros { get; set; }
    }
}
