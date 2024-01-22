using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAppAutores.Filtros
{
    public class MyActionFilter : IActionFilter
    {
        private readonly ILogger<MyActionFilter> logger;

        public MyActionFilter(ILogger<MyActionFilter> logger)
        {
            this.logger = logger;
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("Before excecution");
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation("After excecution");
        }
    }
}
