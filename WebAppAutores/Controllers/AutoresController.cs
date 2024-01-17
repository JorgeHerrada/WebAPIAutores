using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppAutores.Controllers.Entidades;

namespace WebAppAutores.Controllers
{
    // decorators
    [ApiController]         // defines its an api controler
    [Route("api/autores")]  // defines the api route
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AutoresController(ApplicationDbContext context)
        {
            this.context = context;
        }

        // we can have multiple routes pointing to this endpoint
        [HttpGet]               // api/autores -> based on the Route above
        [HttpGet("listado")]    // api/autores/listado -> adding custome one 
        [HttpGet("/listado")]    // /listado -> overwrites the Route when using the prefix '/'
        // the async funcions MUST return a Task<>
        public async Task<ActionResult<List<Autor>>> Get()
        {
            // return all the autors in DB
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        // add '/primero' to route in order to tell apart the TWO GET requests
        [HttpGet("primero")] // api/autores/primero?nombre=felipe
        // after '?' the query params get concatenated
        public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int myValue, [FromQuery] string nombre)
        {
            return await context.Autores.FirstOrDefaultAsync();
        }

        // Returns Autor based on ID received
        [HttpGet("{id:int}/{param2=whatever}/{param3?}")] 
        // ':int' its a restriction on the route valiable
        // '=someValue' sets a default value 
        // '?' makes the param optional, it will be null if not provided
        public async Task<ActionResult<Autor>> Get(int id, string param2, string param3)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null) {
                return NotFound();
            }
            return autor;
        }

        // Returns Autor based on Nombre
        [HttpGet("{nombre}")] 
        public async Task<ActionResult<Autor>> Get([FromRoute]string nombre)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));

            if (autor == null) {
                return NotFound();
            }
            return autor;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Autor autor)
        {
            // autor marked to be added but not added yet
            context.Add(autor);

            // save it to the context
            await context.SaveChangesAsync();

            // HTTP 200 code
            return Ok(); 
        }

        // param gets concatenated to the api route
        [HttpPut("{id:int}")] // api/autores/{id}
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            // IDs don't match?
            if (autor.Id != id)
            {
                return BadRequest("The author's id does not match the id in the URL");
            }

            // author with the received ID exist in the DB?
            var authorExist = await context.Autores.AnyAsync(x => x.Id == id);
            if (!authorExist)
            {
                return NotFound();
            }

            // prepare the update
            context.Update(autor);

            // execute the update and save it in DB
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:int}")] // api/autores/{id}
        public async Task<ActionResult> Delete(int id)
        {
            // author with the received ID exist in the DB
            var authorExist = await context.Autores.AnyAsync(x => x.Id == id);
            if (!authorExist)
            {
                return NotFound();
            }

            // prepare deletion of the author with the assigned ID
            context.Remove(new Autor() { Id = id }); ;
            // execute DB update
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
