using AqLife.Shared.IView;

namespace AqLife.Application.Corpus
{
    public record CorpusDto(
         Guid UID,
         string CorpusContent,//无限制
         string CreateDate
        ) : IEntityDto;
}
