using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppAutores.Controllers.Entidades;

namespace WebAppAutores.Controllers
{
    // decorators
    [ApiController]         // defines its an api controler
    [Route("api/libros")]   // defines the api route
    public class LibrosController: ControllerBase
    {
        private readonly ApplicationDbContext context;

        public LibrosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("{id:int}")] // api/libros/{id}
        public async Task<ActionResult<Libro>> Get(int id)
        {
            return await context.Libros.Include(x => x.Autor).FirstOrDefaultAsync(x => x.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Libro libro)
        {
            // the books author exists?
            var autorExist = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);
            if (!autorExist) {
                return BadRequest($"The autor wit ID: {libro.AutorId} does not exists");
            }

            // prepare changes
            context.Add(libro);
            
            // save changes to DB
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
