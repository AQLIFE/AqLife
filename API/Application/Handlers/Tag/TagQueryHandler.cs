using MediatR;
using MyLife.Domain.Command;
using MyLife.Service.EntityService;
using MyLife.Service.MapperService;
using MyLife.Shared.IView;

namespace MyLife.Application.Handlers.Tag
{
    public class TagQueryHandler(TagServices services, TagMapper tagMapper) : IRequestHandler<TagQuery, IEnumerable<TagDto>?>
    {
        public async Task<IEnumerable<TagDto>?> Handle(TagQuery query, CancellationToken ct)
        {
            var entites = await services.Search(ct, query.UID, query.Tag);
            return entites?.Select(e => tagMapper.ToDto(e!)) ?? [];
        }
    }
}
