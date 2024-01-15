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
    }
}
