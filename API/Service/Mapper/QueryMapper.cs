using MyLife.Domain.Command;
using MyLife.Service.Interfaces;
using Riok.Mapperly.Abstractions;

namespace MyLife.Service.Mapper
{
    [Mapper]
    public partial class QueryMapper
    {
        [MapProperty(nameof(TodoQuery.Desc), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(TodoQuery query);

        [MapProperty(nameof(TagQuery.Tag), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(TagQuery query);

        [MapProperty(nameof(FileQuery.Title), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(FileQuery query);

        //[MapProperty(nameof(AccountQuery.), nameof(EntitySearchCriteria.Keyword))]
        //public partial EntitySearchCriteria ToCriteria(AccountQuery query);
    }
}
