using Microsoft.EntityFrameworkCore;
using WebAppAutores.Controllers.Entidades;

namespace WebAppAutores
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // define composed primary key on AutorLibro entity
            // using an object with both Autor and Libro keys
            modelBuilder.Entity<AutorLibro>().HasKey(al => new {al.AutorId, al.LibroId});
        }

        // creacion de tabla Autores con las propiedades de la clase Autor 
        public DbSet<Autor> Autores { get; set; }

        // creacion de tabla Libros con propiedades de Libro
        public DbSet<Libro> Libros { get; set; }

        // Comentarios Table
        public DbSet<Comentario> Comentarios { get; set; }

        // AutoresLibros Table
        public DbSet<AutorLibro> AutoresLibros { get; set; }
    }
}
