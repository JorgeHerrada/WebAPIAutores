using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppAutores.Controllers.Entidades;
using WebAppAutores.Servicios;

namespace WebAppAutores.Controllers
{
    // decorators
    // [Authorize] // ALL endpoints protected, needs authentication
    [ApiController]         // defines its an api controler
    [Route("api/autores")]  // defines the api route
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServicio servicio;
        private readonly ServicioTransient servicioTransient;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioSingleton servicioSingleton;
        private readonly ILogger<AutoresController> logger;

        public AutoresController(
            ApplicationDbContext context,
            IServicio servicio,
            ServicioTransient servicioTransient,
            ServicioScoped servicioScoped,
            ServicioSingleton servicioSingleton,
            ILogger<AutoresController> logger
        )
        {
            this.context = context;
            this.servicio = servicio;
            this.servicioTransient = servicioTransient;
            this.servicioScoped = servicioScoped;
            this.servicioSingleton = servicioSingleton;
            this.logger = logger;
        }

        [HttpGet("GUID")]
        [ResponseCache(Duration = 10)] // 10 seconds cache will response the same during that time
        public ActionResult ObtenerGuids()
        {
            return Ok(new
            {
                // Transient will be different on each time
                AutoresController_Transient = servicioTransient.Guid,
                ServicioA_Transient = servicio.ObtenerTransient(),

                // Scoped will be the same cause is the same context
                // it will only change on the next HTTP request 
                AutoresController_Scoped = servicioScoped.Guid,
                ServicioA_Scoped = servicio.ObtenerScoped(),
                
                // Singleton will be always the same for all contexts and requests
                AutoresController_Singleton = servicioSingleton.Guid,
                ServicioA_Singleton = servicio.ObtenerSingleton()
            });
        }

        // we can have multiple routes pointing to this endpoint
        [HttpGet]               // api/autores -> based on the Route above
        [HttpGet("listado")]    // api/autores/listado -> adding custome one 
        [HttpGet("/listado")]    // /listado -> overwrites the Route when using the prefix '/'
        [ResponseCache(Duration = 10)] // 10 seconds cache will response the same during that time
        [Authorize] // endpoint protected, needs authentication
        // the async funcions MUST return a Task<>
        public async Task<ActionResult<List<Autor>>> Get()
        {
            logger.LogInformation("Information: Getting the Autores");
            logger.LogWarning("Warning: Getting the Autores");

            // return all the autors in DB
            servicio.RealizarTarea();
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
            var autorExist = await context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);

            if (autorExist)
            {
                return BadRequest($"The Autor {autor.Nombre} already exists");
            }

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
