using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppAutores.Controllers.Entidades;
using WebAppAutores.DTOs;

namespace WebAppAutores.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")] // Comentarios depends on Libro existence
    public class ComentariosController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ComentariosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            // the libro exist?
            var libroExist = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!libroExist)
            {
                return NotFound($"The Libro with ID: {libroId} does not exist");
            }

            // get the associated comentarios
            var comentarios = await context.Comentarios
                .Where(comentarioDB => comentarioDB.LibroId == libroId).ToListAsync();

            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id:int}", Name = "GetComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetComentarioPorID(int libroId, int id)
        {
            var comentario = await context.Comentarios
                .Where(libroDB => libroDB.LibroId == libroId)  // necessary? 
                .FirstOrDefaultAsync(comentarioDB => comentarioDB.Id == id);

            if (comentario == null) { return NotFound(); }

            return mapper.Map<ComentarioDTO>(comentario);
        }

        // post commet to a book
        [HttpPost]
        public async Task<ActionResult> Post(int libroId, ComentarioCreationDTO comentarioCreationDTO)
        {
            // the libro exist?
            var libroExist = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!libroExist)
            {
                return NotFound($"The Libro with ID: {libroId} does not exist");
            }

            // map from api interfase to DB entity type 
            var comentario = mapper.Map<Comentario>(comentarioCreationDTO);

            comentario.LibroId = libroId;

            // comentario MARKED to be added but not added yet
            context.Add(comentario);

            // INSERT changes and save it to the context
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            // HTTP 200 code
            return CreatedAtRoute("GetComentario", new { id = comentario.Id, libroId }, comentarioDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int libroId, int id, ComentarioCreationDTO comentarioCreationDTO)
        {
            // the libro exist?
            var libroExist = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!libroExist)
            {
                return NotFound($"The Libro with ID: {libroId} does not exist");
            }

            var comentarioExist = await context.Comentarios.AnyAsync(comentarioDB => comentarioDB.Id == id);
            if (!comentarioExist)
            {
                return NotFound($"The Comentario with ID: {id} does not exist");
            }

            var comentario = mapper.Map<Comentario>(comentarioCreationDTO);
            comentario.Id = id;
            comentario.LibroId = libroId;

            context.Update(comentario);
            await context.SaveChangesAsync();

            return NoContent(); // 204

        }
    }
}
