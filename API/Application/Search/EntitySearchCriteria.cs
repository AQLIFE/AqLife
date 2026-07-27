using MyLife.Application.Abstractions.Search;

namespace MyLife.Application.Search
{
    public readonly record struct EntitySearchCriteria(Guid? UID, string? Keyword) : ISearchCriteria;
}
