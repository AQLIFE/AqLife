using AqLife.Application.Abstractions.Search;

namespace AqLife.Application.Search
{
    public readonly record struct EntitySearchCriteria(Guid? UID, string? Keyword) : ISearchCriteria;
    public readonly record struct TodoSearchCriteria(Guid? UID, string? Keyword,Guid? PlanId) : ISearchCriteria;
    public readonly record struct FavoriteSearchCriteria(Guid? UID = null, string? Keyword = null, Guid? CategoryId = null, Guid? TagId = null) : ISearchCriteria;

    public readonly record struct FileSearchCriteria(Guid? UID,string? Keyword,Guid? CategoryUID) : ISearchCriteria;

}
