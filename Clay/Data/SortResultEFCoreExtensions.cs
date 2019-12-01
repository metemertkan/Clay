using System;
using System.Linq;
using Clay.Data.Sort;

namespace Clay.Data
{
    public static class SortResultEFCoreExtensions
    {
        public static IOrderedQueryable<TSource> ApplySortDirection<TSource, TKey>(this IQueryable<TSource> source, System.Linq.Expressions.Expression<Func<TSource, TKey>> keySelector, bool descending)
        {
            if (!source.Expression.Type.Name.Contains("IOrderedQueryable"))
                return descending ? source.OrderByDescending(keySelector) : source.OrderBy(keySelector);

            var orderedSource = (IOrderedQueryable<TSource>)source;

            return descending ? orderedSource.ThenByDescending(keySelector) : orderedSource.ThenBy(keySelector);

        }
    }
}