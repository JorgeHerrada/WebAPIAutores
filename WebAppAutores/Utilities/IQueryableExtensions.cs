using WebAppAutores.DTOs;

namespace WebAppAutores.Utilities
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO paginationDTO)
        {
            return queryable
                .Skip((paginationDTO.Page - 1) * paginationDTO.ItemsPerPage)
                .Take(paginationDTO.ItemsPerPage);
        }
    }
}
