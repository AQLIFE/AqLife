using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Tag.Handler
{
    // 分类查询处理器
    public class CategoryQueryHandler(IApplicationDbContext dbContext, CategoryMappingService categoryMapper, IHttpContextAccessor httpContext) : IRequestHandler<CategoryQuery, PageResult<CategoryDto>?>
    {
        bool Isvalid => httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        public async Task<PageResult<CategoryDto>?> Handle(CategoryQuery query, CancellationToken ct)
        {
            IQueryable<CategoryEntity> queryable = Isvalid ? dbContext.Categories.Include(e => e.ParentCategory) : dbContext.Categories.Include(e => e.Files).ThenInclude(file => file.PublishMeta).Include(e => e.ParentCategory);
            IEnumerable<CategoryDto> result = queryable.Select(categoryMapper.ToDto).OrderByDescending(e => e.CategoryCount).Skip(0).Take(10);// 这样会得到所有分类及其文件数量，但不包括文件的详细信息
            return new PageResult<CategoryDto>([.. result], 1, 10, false);// 写死返回总量,只显示前10个数量集
        }
    }
}
