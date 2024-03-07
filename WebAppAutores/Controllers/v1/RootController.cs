using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppAutores.DTOs;

namespace WebAppAutores.Controllers.v1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [AllowAnonymous]
        [HttpGet(Name = "obtener-root")]
        public async Task<ActionResult<IEnumerable<DatoHATEOSDTO>>> Get()
        {
            var datosHateoas = new List<DatoHATEOSDTO>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

            datosHateoas.Add(new DatoHATEOSDTO(
                enlace: Url.Link("obtener-root", new { }),
                descripcion: "self",
                metodo: "GET"
            ));

            datosHateoas.Add(new DatoHATEOSDTO(
                enlace: Url.Link("obtener-autores", new { }),
                descripcion: "autores",
                metodo: "GET"
            ));

            if (esAdmin.Succeeded)
            {
                datosHateoas.Add(new DatoHATEOSDTO(
                    enlace: Url.Link("crear-autor", new { }),
                    descripcion: "autor-crear",
                    metodo: "POST"
                ));

                datosHateoas.Add(new DatoHATEOSDTO(
                    enlace: Url.Link("crear-libro", new { }),
                    descripcion: "libro-crear",
                    metodo: "POST"
                ));
            }

            return datosHateoas;
        }
    }
}
