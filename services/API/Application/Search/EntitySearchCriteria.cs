using AqLife.Application.Abstractions.Search;

namespace AqLife.Application.Search
{
    public readonly record struct EntitySearchCriteria(Guid? UID, string? Keyword) : ISearchCriteria;
}
