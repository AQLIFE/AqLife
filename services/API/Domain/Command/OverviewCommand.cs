using AqLife.Domain.CommandInterface;
using AqLife.Shared.IView;

namespace AqLife.Domain.Command
{
    public record OverviewQuery(int RecentCount = 5) : IQuery<OverviewDto>;
}
