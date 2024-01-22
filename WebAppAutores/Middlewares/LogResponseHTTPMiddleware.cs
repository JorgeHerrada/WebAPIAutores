using Microsoft.Extensions.Logging;

namespace WebAppAutores.Middlewares
{
    public static class LogResponseHTTPMiddlewareExtension
    {
        public static IApplicationBuilder UseLogResponseHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LogResponseHTTPMiddleware>();
        }
    }


    public class LogResponseHTTPMiddleware
    {
        // RequestDelegate calls the next middleware on pipeline
        private readonly RequestDelegate next;
        private readonly ILogger<LogResponseHTTPMiddleware> logger;

        public LogResponseHTTPMiddleware(RequestDelegate next,
            ILogger<LogResponseHTTPMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        // Invoke or InvokeAsync: Task (HTTPContext)
        public async Task InvokeAsync(HttpContext context)
        {
            // loggs all the HTTP responses
            using (var memoryStream = new MemoryStream())
            {
                var cuerpoOriginalRespuesta = context.Response.Body;
                context.Response.Body = memoryStream;

                await next(context); // starts next middleware and wait for its response

                memoryStream.Seek(0, SeekOrigin.Begin);
                string response = new StreamReader(memoryStream).ReadToEnd();

                memoryStream.Seek(0, SeekOrigin.Begin);

                await memoryStream.CopyToAsync(cuerpoOriginalRespuesta);
                context.Response.Body = cuerpoOriginalRespuesta;

                logger.LogInformation(response);
            }
        }
    }
}
