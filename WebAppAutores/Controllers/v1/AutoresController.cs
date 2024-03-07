using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppAutores.Controllers.Entidades;
using WebAppAutores.DTOs;
using WebAppAutores.Utilities;

namespace WebAppAutores.Controllers.v1
{
    // decorators
    // ALL endpoints protected, needs authentication
    // access only with EsAdmin field in claim
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    [ApiController]         // defines its an api controler
    [Route("api/v1/autores")]  // defines the api route
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(
            ApplicationDbContext context,
            IMapper mapper,
            IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }

        // we can have multiple routes pointing to this endpoint
        [HttpGet(Name = "obtener-autores-v1")] // api/autores -> based on the Route above
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // protect endpoint
        [AllowAnonymous] // Authentication not needed on this endpoint
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get() // async MUST return Task<>
        {
            var autores = await context.Autores.ToListAsync();
            var dto = mapper.Map<List<AutorDTO>>(autores);
            return dto;
        }

        // Returns Autor based on ID received
        [HttpGet("{id:int}", Name = "obtener-autor-por-id-v1")] // ':int' its a restriction on the route valiable
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor = await context.Autores
                .Include(autorDB => autorDB.AutoresLibros)  // access AutorLibro table
                .ThenInclude(autorLibroDB => autorLibroDB.Libro)    // access Libros list in AutorLibroTable
                .FirstOrDefaultAsync(autorDB => autorDB.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<AutorDTOConLibros>(autor);

            return dto;
        }

        // Returns Autor based on Nombre, list with all that matches
        [HttpGet("{nombre}", Name = "obtener-autor-por-nombre-v1")]
        public async Task<ActionResult<List<AutorDTO>>> GetByName([FromRoute] string nombre)
        {
            var autores = await context.Autores.Where(autorDB => autorDB.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpPost(Name = "crear-autor-v1")]
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

            var autorDTO = mapper.Map<AutorDTO>(autor); // mapp to AutorDTO

            // HTTP 201 - Created, shares route to find autor and return it
            return CreatedAtRoute("obtener-autor-por-id-v1", new { id = autor.Id }, autorDTO);
        }

        // param gets concatenated to the api route
        [HttpPut("{id:int}", Name = "actualizar-autor-v1")] // api/autores/{id}
        public async Task<ActionResult> Put(AutorCreationDTO autorCreationDTO, int id)
        {
            // author with the received ID exist in the DB?
            var authorExist = await context.Autores.AnyAsync(x => x.Id == id);
            if (!authorExist)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorCreationDTO);
            autor.Id = id;

            // prepare the update
            context.Update(autor);

            // execute the update and save it in DB
            await context.SaveChangesAsync();

            return NoContent(); // 204
        }

        [HttpDelete("{id:int}", Name = "borrar-autor-v1")] // api/autores/{id}
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

            return NoContent();
        }
    }
}
