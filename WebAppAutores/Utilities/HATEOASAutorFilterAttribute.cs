using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAppAutores.DTOs;
using WebAppAutores.Servicios;

namespace WebAppAutores.Utilities
{
    public class HATEOASAutorFilterAttribute : HATEOASFiltroAttribute
    {
        private readonly GeneradorEnlaces generadorEnlaces;

        public HATEOASAutorFilterAttribute(GeneradorEnlaces generadorEnlaces)
        {
            this.generadorEnlaces = generadorEnlaces;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var mustInclude = MustIncludeHATEOAS(context);

            if (!mustInclude)
            {
                await next();
                return;
            }

            var resultado = context.Result as ObjectResult;
            var modelo = resultado.Value as AutorDTO ?? 
                throw new ArgumentNullException("An instance of AutorDTO was expected");

            await generadorEnlaces.GenerarEnlaces(modelo);

            await next();
        }
    }
}
