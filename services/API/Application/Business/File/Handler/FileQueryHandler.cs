using AqLife.Application.Abstractions.FileStorage;
using AqLife.Application.Business.File.Search;
using AqLife.Domain.Command;
using AqLife.Shared.IView;
using MediatR;
using AqLife.Application.Business.File;

namespace AqLife.Application.Business.File.Handler
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
