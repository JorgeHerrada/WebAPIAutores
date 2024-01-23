using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppAutores.Controllers.Entidades;
using WebAppAutores.DTOs;

namespace WebAppAutores.Controllers
{
    // decorators
    // [Authorize] // ALL endpoints protected, needs authentication
    [ApiController]         // defines its an api controler
    [Route("api/autores")]  // defines the api route
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        // we can have multiple routes pointing to this endpoint
        [HttpGet] // api/autores -> based on the Route above
        public async Task<ActionResult<List<AutorDTO>>> Get() // async MUST return Task<>
        {
            // temp comment, until x.Libros is accessible again
            //return await context.Autores.Include(x => x.Libros).ToListAsync(); 
            var autores = await context.Autores.ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);
        }

        // Returns Autor based on ID received
        [HttpGet("{id:int}")] // ':int' its a restriction on the route valiable
        public async Task<ActionResult<AutorDTO>> Get(int id)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(autorDB => autorDB.Id == id);

            if (autor == null) {
                return NotFound();
            }

            return mapper.Map<AutorDTO>(autor);
        }

        // Returns Autor based on Nombre, list with all that matches
        [HttpGet("{nombre}")] 
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute]string nombre)
        {
            var autores = await context.Autores.Where(autorDB => autorDB.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorCreationDTO autorCreationDTO)
        {
            var autorExist = await context.Autores.AnyAsync(autorDB => autorDB.Nombre == autorCreationDTO.Nombre);

            if (autorExist)
            {
                return BadRequest($"The Autor {autorCreationDTO.Nombre} already exists");
            }

            // map the DTO class to the Autor class that can be sent to the DATABASE
            var autor = mapper.Map<Autor>(autorCreationDTO);

            // autor MARKED to be added but not added yet
            context.Add(autor);

            // INSERT changes and save it to the context
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
