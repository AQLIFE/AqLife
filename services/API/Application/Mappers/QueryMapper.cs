using AqLife.Application.Search;
using AqLife.Domain.Command;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Mappers
{
    [Mapper]
    public partial class QueryMapper
    {
        [MapperIgnoreSource(nameof(TodoQuery.IsTree))]
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
