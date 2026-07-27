using MyLife.Application.Search;
using MyLife.Domain.Command;
using Riok.Mapperly.Abstractions;

namespace MyLife.Application.Mappers
{
    [Mapper]
    public partial class QueryMapper
    {
        [MapProperty(nameof(TodoQuery.Desc), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(TodoQuery query);

        [MapProperty(nameof(TagQuery.Tag), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(TagQuery query);

        [MapperIgnoreTarget(nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(AccountQuery query);


        [MapProperty(nameof(FileQuery.Title), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(FileQuery query);

        [MapProperty(nameof(CorpusQuery.Content), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(CorpusQuery query);
    }
}
