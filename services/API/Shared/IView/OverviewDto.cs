using System.Collections.Generic;

namespace AqLife.Shared.IView
{
    public record OverviewDto(
        int DraftCount,
        int ScheduledCount,
        int PublishedCount,
        IEnumerable<FileDto> RecentFiles
    );
}
