namespace AqLife.Application.Abstractions.Search
{
    public interface ISearchCriteria
    {
        Guid? UID { get; init; }
        string? Keyword { get; init; }
    }
}
