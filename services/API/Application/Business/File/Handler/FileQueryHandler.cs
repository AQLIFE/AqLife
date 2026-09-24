using AqLife.Application.Abstractions.FileStorage;
using AqLife.Application.Business.File.Search;
using AqLife.Domain.Command;
using AqLife.Shared.IView;
using MediatR;
using AqLife.Application.Business.File;
using AqLife.Application.Business.File.Service;
using AqLife.Application.Mappers;
using AqLife.Domain.Entities.File;

namespace AqLife.Application.Business.File.Handler
{
    public class FileQueryHandler(FileSearch search,PageResultMapper<FileMetaEntity,FileDto> mapper, FileReader fileReader) : IRequestHandler<FileQuery, PageResult<FileDto>>
    {
        public async Task<PageResult<FileDto>> Handle(FileQuery query, CancellationToken ct)
        {
            var result = await search.SearchPageAsync(query, ct);

            foreach (var item in result.Items)
            {
                if (item.Extension == ".md")
                {
                    string content = (await fileReader.GetContentAsync(item.StorageKey, ct));
                    item.SetFileIntroduction(content.Length <= 300 ? content : content[..300]);
                }
            }
            return mapper.ToDto(result);
        }
    }
}
