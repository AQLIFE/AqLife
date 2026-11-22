using Core.Search;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities;
using Riok.Mapperly.Abstractions;
using Corpus;

namespace AqLife.Application.Business.Corpus
{
    [Mapper]
    public partial class CorpusMapper : IViewMapper<CorpusEntity, CorpusDto>, ICreateMapper<CorpusEntity, CreateCorpusCommand>,
        ISearchMapper<CorpusQuery, PageResult<CorpusDto>, EntitySearchCriteria>
    {
        [MapperIgnoreSource(nameof(CorpusQuery.Page))]
        [MapperIgnoreSource(nameof(CorpusQuery.PageSize))]
        [MapProperty(nameof(CorpusQuery.Content), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(CorpusQuery query);

        public partial CorpusDto ToDto(CorpusEntity source);

        public CorpusEntity ToEntity(CreateCorpusCommand command) => new(command.Content);

        public static string Convert(DateTimeOffset dateTime) => dateTime.ToString("yyyy-MM-dd");
    }
}
