using AqLife.Application.Abstractions.Search;
using AqLife.Application.Business.File.Search;
using AqLife.Application.Business.File.Service;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.File;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using MediatR;
using Microsoft.Extensions.Options;

namespace AqLife.Application.Business.File.Handler
{
    public class FileQueryHandler(ISearch<FileQuery, FileMetaEntity> search, IOptions<FilePolicyOption> options,FileReader fileReader, PageResultMapper<FileMetaEntity, FileDto> mapper) : IRequestHandler<FileQuery, PageResult<FileDto>>
    {
        public async Task<PageResult<FileDto>> Handle(FileQuery query, CancellationToken ct)
        {
            var result = await search.SearchPageAsync(query, ct);
            if (query.Order == Shared.Options.FileOrder.Latest)
            {
                var items = result.Items.ToList();
                foreach (var item in items)
                {
                    if (options.Value.AllowedBlogExtensions.Contains(item.Extension))
                    {
                        string content = await fileReader.GetContentAsync(item.StorageKey, ct);
                        item.SetFileIntroduction(content.Length <= 300 ? content : content[..300]);
                    }
                }
            }
            return mapper.ToDto(result);
        }
    }
}
