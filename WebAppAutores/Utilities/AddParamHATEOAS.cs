using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebAppAutores.Utilities
{
    public class AddParamHATEOAS : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // only add HATEOAS in GET methods
            if (context.ApiDescription.HttpMethod != "GET")
            {
                return;
            }

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter> ();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "includeHATEOAS",
                In = ParameterLocation.Header,
                Required = false
            });
        }
    }
}
