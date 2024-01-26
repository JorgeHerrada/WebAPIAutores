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
            var libro = await context.Libros
                .Include(libroDB => libroDB.AutoresLibros) // go into AutoresLibros
                .ThenInclude(autorLibroDB => autorLibroDB.Autor)  //  go into Autor
                //.Include(libroDB => libroDB.Comentarios) // Adds the comments using JOIN behind
                .FirstOrDefaultAsync(libroDB => libroDB.Id == id);

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibroDTO>(libro);
        }

        [HttpPost]
        public async Task<ActionResult> Post(LibroCreationDTO libroCreationDTO)
        {

            if (libroCreationDTO.AutoresIds == null)
            {
                return BadRequest("You can not create a Libro without Autor");
            }

            // get a list of autores' Ids that matches the autores' Ids received
            var autoresIds = await context.Autores
                .Where(autorDB => libroCreationDTO.AutoresIds.Contains(autorDB.Id))
                .Select(x => x.Id).ToListAsync();

            // lists length doesn't match?
            if (libroCreationDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("At least one of the Autores sent, does not exist");
            }

            var libro = mapper.Map<Libro>(libroCreationDTO);

            if (libro.AutoresLibros != null)
            {
                for(int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }

            // prepare changes
            context.Add(libro);

            // save changes to DB
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
