using Microsoft.EntityFrameworkCore;
using TouristGuide.Domain.Common;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Infrastructure.Specifications
{
    public class SpecificationEvaluator<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecifications<TEntity, TKey> spec)
        {
            var query = inputQuery;

            // 1. تطبيق الشروط (Where)
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            // 2. تطبيق الجداول المرتبطة (Includes)
            if (spec.IncludeExpressions.Any())
            {
                query = spec.IncludeExpressions.Aggregate(query, (current, include) => current.Include(include));
            }

            // 3. تطبيق الترتيب (OrderBy)
            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            // 4. تطبيق التقسيم (Pagination)
            if (spec.IsPaginated)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            return query;
        }

    }
}