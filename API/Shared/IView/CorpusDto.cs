namespace MyLife.Shared.IView
{
    public record CorpusDto(
         Guid UID,
         string CorpusContent//无限制
        ) : IEntityDto;
}
