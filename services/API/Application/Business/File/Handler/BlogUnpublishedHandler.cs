using AqLife.Application.Abstractions.FileStorage;
using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.File.Search;
using AqLife.Application.Services;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.File;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Business.File.Handler
{
    public class BlogUnpublishedHandler(BlogSearch search,IViewMapper<FileMetaEntity,FileDto> fileMapper,TimeProvider timeProvider) : IRequestHandler<BlogUnpublishQuery, IEnumerable<FileDto>>
    {
        public async Task<IEnumerable<FileDto>> Handle(BlogUnpublishQuery query, CancellationToken ct)
        {
            var result = await search.GetScheduledPostsDueAsync(timeProvider.GetUtcNow(),ct);
            return result.Select(fileMapper.ToDto);
        }
    }
}
