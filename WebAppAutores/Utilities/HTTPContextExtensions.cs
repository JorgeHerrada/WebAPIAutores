using Microsoft.EntityFrameworkCore;

namespace WebAppAutores.Utilities
{
    public static class HTTPContextExtensions
    {
        public async static Task InsertPaginationParamsInHeader<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if (httpContext == null) { throw new ArgumentNullException(nameof(httpContext)); }

            double count = await queryable.CountAsync();
            httpContext.Response.Headers.Add("totalItems", count.ToString());
        }
    }
}
