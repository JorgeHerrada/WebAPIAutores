using Microsoft.AspNetCore.Mvc;
using WebAppAutores.DTOs;

namespace WebAppAutores.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "obtener-root")]
        public ActionResult<IEnumerable<DatoHATEOSDTO>> Get()
        {
            var datosHateoas = new List<DatoHATEOSDTO>();

            datosHateoas.Add(
                new DatoHATEOSDTO(
                    enlace: Url.Link("obtener-root", new { }),
                    descripcion: "self",
                    metodo: "GET"
                )
            );

            return datosHateoas;
        }
    }
}
