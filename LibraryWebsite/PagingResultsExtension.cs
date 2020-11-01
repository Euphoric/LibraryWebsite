using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebsite
{
    public static class PagingResultsExtension
    {
        public static async Task<PagingResult<T>> CreatePaging<T>(this IQueryable<T> query, int limit, int page)
        {
            T[] items =
                await
                query
                .Skip(limit * page)
                .Take(limit)
                .ToArrayAsync();

            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)limit);

            return new PagingResult<T>(items, page, totalPages, totalCount);
        }

        public static PagingResult<TTo> Select<TFrom, TTo>(this PagingResult<TFrom> paging, Func<TFrom, TTo> selector)
        {
            return new PagingResult<TTo>(
                paging.Items.Select(selector).ToArray(),
                paging.CurrentPage,
                paging.TotalPages,
                paging.TotalCount);
        }

        public static async Task<PagingResult<TTo>> Select<TFrom, TTo>(this Task<PagingResult<TFrom>> paging, Func<TFrom, TTo> selector)
        {
            return (await paging).Select(selector);
        }
    }
}
