using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
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

        [HttpGet("{id:int}", Name = "obtener-libro")] // api/libros/{id}
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            var libro = await context.Libros
                .Include(libroDB => libroDB.AutoresLibros) // go into AutoresLibros
                .ThenInclude(autorLibroDB => autorLibroDB.Autor)  //  go into Autor
                //.Include(libroDB => libroDB.Comentarios) // Adds the comments using JOIN behind
                .FirstOrDefaultAsync(libroDB => libroDB.Id == id);

            if (libro == null) { return NotFound(); }

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibroDTOConAutores>(libro);
        }

        [HttpPost(Name = "crear-libro")]
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

            SortAutores(libro);

            // prepare changes
            context.Add(libro);

            // save changes to DB
            await context.SaveChangesAsync();

            // map to LibroDTO
            var libroDTO = mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("obtener-libro", new {id  = libro.Id}, libroDTO);
        }

        [HttpPut("{id:int}", Name = "actualizar-libro")]
        public async Task<ActionResult> Put(int id, LibroCreationDTO libroCreationDTO)
        {
            // Autores exists?
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

            // get the Libro from DB based on the received id, also bring the AutoresLibros table
            var libroDB = await context.Libros
                .Include(x => x.AutoresLibros)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libroDB == null) { return NotFound(); }

            // maps from libroCreationDTO to libroDB
            // updating libroDB keeping the same instance created above
            // so no need to do the usual context.update(<updatedInstance>)
            libroDB = mapper.Map(libroCreationDTO, libroDB);

            SortAutores(libroDB);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}", Name ="patch-libro")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var libroDB = await context.Libros.FirstOrDefaultAsync(libroDB => libroDB.Id == id);

            if (libroDB == null) { return NotFound(); }

            var libroPatchDTO = mapper.Map<LibroPatchDTO>(libroDB);

            // apply to libroDTO the changes received in the patchDocument
            patchDocument.ApplyTo(libroPatchDTO, ModelState);

            var validData = TryValidateModel(libroPatchDTO); // validate contrains

            if (! validData)
            {
                return BadRequest();
            }

            mapper.Map(libroPatchDTO, libroDB); // map from DTO to DB

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "borrar-libro")] // api/libros/{id}
        public async Task<ActionResult> Delete(int id)
        {
            // author with the received ID exist in the DB
            var libroExist = await context.Libros.AnyAsync(libroDB => libroDB.Id == id);
            if (!libroExist)
            {
                return NotFound();
            }

            // prepare deletion of the author with the assigned ID
            context.Remove(new Libro() { Id = id }); ;
            // execute DB update
            await context.SaveChangesAsync();

            return NoContent();
        }

        // make sure the autores are sorted same as the user sent them
        private void SortAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }
    }
}
