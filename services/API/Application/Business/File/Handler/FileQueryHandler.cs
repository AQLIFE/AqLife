using MediatR;
using MyLife.Application.Abstractions.FileStorage;
using MyLife.Application.Business.File.Search;
using MyLife.Domain.Command;
using MyLife.Shared.IView;

namespace MyLife.Application.Business.File.Handler
{
    public class FileQueryHandler(FileSearch search, FileMapper mapper, IFileStorage fileStorage) : IRequestHandler<FileQuery, IEnumerable<FileDto>>
    {
        public async Task<IEnumerable<FileDto>> Handle(FileQuery query, CancellationToken ct)
        {
            var result = await search.SearchAsync(query, ct);

            foreach (var item in result)
            {
                if (item.Extension == ".md")
                {
                    string content = (await fileStorage.GetContent(item.StorageName));
                    item.SetFileIntroduction(content.Length <= 300 ? content : content[..300]);
                }
            }
            return result?.Select(e => mapper.ToDto(e)) ?? [];
        }
    }
}
