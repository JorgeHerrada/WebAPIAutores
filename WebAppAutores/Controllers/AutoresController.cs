using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppAutores.Controllers.Entidades;

namespace WebAppAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AutoresController(ApplicationDbContext context)
        {
            this.context = context;
        }

        // the async funcions MUST return a Task<>
        [HttpGet]
        public async Task<ActionResult<List<Autor>>> Get()
        {
            // return all the autors in DB
            return await context.Autores.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Autor autor)
        {
            // autor marked to be added but not added yet
            context.Add(autor);

            // adding what is added in the context
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
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
