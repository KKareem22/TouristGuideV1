using System.Linq.Expressions;
using TouristGuide.Domain.Common;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Application.Specification
{
    public class BaseSpecifications<TEntity, TKey> : ISpecifications<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        public Expression<Func<TEntity, bool>>? Criteria { get; set; }
        public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = new();
        public Expression<Func<TEntity, object>>? OrderBy { get; set; }
        public Expression<Func<TEntity, object>>? OrderByDescending { get; set; }
        public int Skip { get; private set; }
        public int Take { get; private set; }
        public bool IsPaginated { get; private set; }

        ICollection<Expression<Func<TEntity, object>>> ISpecifications<TEntity, TKey>.IncludeExpressions => IncludeExpressions;

        public BaseSpecifications()
        {
        }

        public BaseSpecifications(Expression<Func<TEntity, bool>> criteria)
        {
            Criteria = criteria;
        }

        protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
        {
            IncludeExpressions.Add(includeExpression);
        }

        protected void ApplyPagination(int skip, int take)
        {
            IsPaginated = true;
            Skip = skip;
            Take = take;
        }
    }
}
