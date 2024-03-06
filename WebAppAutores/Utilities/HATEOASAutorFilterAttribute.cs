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

            //var modelo = resultado.Value as AutorDTO ?? 
            //    throw new ArgumentNullException("An instance of AutorDTO was expected");
            // refactor prev lines to include list:
            var autorDTO = resultado.Value as AutorDTO;

            if (autorDTO == null)
            {
                var autoresDTO = resultado.Value as List<AutorDTO> ??
                    throw new ArgumentNullException("An instance of AutorDTO or List<AutorDTO> was expected");

                autoresDTO.ForEach(async autorDTO => await generadorEnlaces.GenerarEnlaces(autorDTO));

                resultado.Value = autoresDTO;
            }
            else
            {
                await generadorEnlaces.GenerarEnlaces(autorDTO);
            }
            await next();
        }
    }
}
