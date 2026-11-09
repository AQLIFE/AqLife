namespace AqLife.Shared.IView
{
    public record PageResult<TData>(List<TData> Items, int Page, int PageSize, bool HasMore = true);
}
