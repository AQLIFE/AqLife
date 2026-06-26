using Microsoft.AspNetCore.Http;
using MyLife.Shared.DTOs;

namespace MyLife.Shared.Tools
{
    public static class GetFiles
    {
        public static IEnumerable<IFormFile> GetIEnumerableFiles(this IEnumerable<SubscriptionFullDto> dtos)
            => dtos.Where(x => x.NewIconFile != null).Select(e => e.NewIconFile).AsEnumerable()!;
    }
}
