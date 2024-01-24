using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppAutores.Controllers.Entidades;
using WebAppAutores.DTOs;

namespace WebAppAutores.Controllers
{
    // decorators
    [ApiController]         // defines its an api controler
    [Route("api/libros")]   // defines the api route
    public class LibrosController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}")] // api/libros/{id}
        public async Task<ActionResult<LibroDTO>> Get(int id)
        {
            //return await context.Libros.Include(x => x.Autor).FirstOrDefaultAsync(x => x.Id == id);
            var libro = await context.Libros.FirstOrDefaultAsync(libroDB => libroDB.Id == id);

            return mapper.Map<LibroDTO>(libro);
        }

        [HttpPost]
        public async Task<ActionResult> Post(LibroCreationDTO libroCreationDTO)
        {
            //// the books author exists?
            //var autorExist = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);
            //if (!autorExist)
            //{
            //    return BadRequest($"The autor wit ID: {libro.AutorId} does not exists");
            //}

            var libro = mapper.Map<Libro>(libroCreationDTO);

            // prepare changes
            context.Add(libro);

            // save changes to DB
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
