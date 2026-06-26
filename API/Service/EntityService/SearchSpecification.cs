using System.Linq.Expressions;

namespace MyLife.Service.EntityService
{
    public class SearchSpecification<TSource>
    {
        public Expression<Func<TSource, bool>>? Criteria { get; private set; }

        public void ApplyCriteria(Expression<Func<TSource, bool>> criteria)
        {
            Criteria = criteria;
        }
    }
}
