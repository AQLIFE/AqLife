using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
