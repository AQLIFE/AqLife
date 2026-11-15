using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using Riok.Mapperly.Abstractions;

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
