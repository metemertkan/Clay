using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Clay.Data.Pagination
{
    public static class PagedResultEFCoreExtensions
    {
        public static async Task<PagedResult<T>> GetPaged<T>(this IQueryable<T> query, PagedModel pagedModel)
        {
            var result = new PagedResult<T>
            {
                CurrentPage = pagedModel.Page,
                PageSize = pagedModel.PageSize,
                RowCount = query.Count()
            };

            var pageCount = (double)result.RowCount / pagedModel.PageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (pagedModel.Page - 1) * pagedModel.PageSize;
            result.Results = await query.Skip(skip).Take(pagedModel.PageSize).ToListAsync();

            return result;
        }
    }
}