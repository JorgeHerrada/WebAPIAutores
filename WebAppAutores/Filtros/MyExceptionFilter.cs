using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAppAutores.Filtros
{
    public class MyExceptionFilter: ExceptionFilterAttribute
    {
        private readonly ILogger<MyExceptionFilter> logger;

        public MyExceptionFilter(ILogger<MyExceptionFilter> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            // filter logs the exception and message
            logger.LogError(context.Exception, context.Exception.Message);

            base.OnException(context);
        }
    }
}
