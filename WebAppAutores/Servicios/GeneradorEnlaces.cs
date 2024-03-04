using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using WebAppAutores.DTOs;

namespace WebAppAutores.Servicios
{
    public class GeneradorEnlaces
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public GeneradorEnlaces(
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor)
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper BuildUrlHelper()
        {
            var factory = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();

            return factory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        private async Task<bool> IsAdmin()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var result = await authorizationService.AuthorizeAsync(httpContext.User, "esAdmin");

            return result.Succeeded;
        }

        public async Task GenerarEnlaces(AutorDTO autorDTO)
        {
            var Url = BuildUrlHelper();
            var isAdmin = await IsAdmin();

            autorDTO.Enlaces.Add(new DatoHATEOSDTO(
                enlace: Url.Link("obtener-autor-por-id", new { id = autorDTO.Id }),
                descripcion: "self",
                metodo: "GET"
            ));

            if (isAdmin)
            {
                autorDTO.Enlaces.Add(new DatoHATEOSDTO(
                    enlace: Url.Link("actualizar-autor", new { id = autorDTO.Id }),
                    descripcion: "autor-actualizar",
                    metodo: "PUT"
                ));
                autorDTO.Enlaces.Add(new DatoHATEOSDTO(
                    enlace: Url.Link("borrar-autor", new { id = autorDTO.Id }),
                    descripcion: "autor-borrar",
                    metodo: "DELETE"
                ));
            }
        }
    }
}
