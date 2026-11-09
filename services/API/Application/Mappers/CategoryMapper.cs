using AqLife.Application.Abstractions.Mapper;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Mappers
{
    [Mapper]
    public partial class CategoryMapper(IOptions<FilePolicyOption> options) : IViewMapper<CategoryEntity, CategoryDto>
    {
        string[] AllowedExtensions => options.Value.AllowedBlogExtensions;

        [MapperIgnoreSource(nameof(CategoryEntity.ParentId))]
        [MapProperty(nameof(CategoryEntity.Files.Count), nameof(CategoryDto.CategoryCount))]
        [MapProperty(nameof(CategoryEntity.ParentCategory.CategoryName), nameof(CategoryDto.ParentName))]
        public partial CategoryDto ToDto(CategoryEntity source);
    }


    public class CategoryMappingService(CategoryMapper categoryMapper, IHttpContextAccessor httpContext, IOptions<FilePolicyOption> options)
        : IViewMapper<CategoryEntity, CategoryDto>
    {
        bool IsValid => httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        string[] AllowedExtensions => options.Value.AllowedBlogExtensions;

        public CategoryDto ToDto(CategoryEntity source)
        => IsValid ? categoryMapper.ToDto(source) : source.Desensitize(AllowedExtensions);
    }

    public static class CategoryMapperExtensions
    {
        /// <summary>
        /// 脱敏处理 CategoryEntity : 基于会话和身份上下文
        /// </summary>
        /// <param name="source">必须完成 include ( source.Files.PublishMeta ,source.ParentCategory  ) ,否则相关参数脱敏后为空</param>
        /// <param name="allowedExtensions">预期查询的文件类型</param>
        /// <returns></returns>
        public static CategoryDto Desensitize(this CategoryEntity source, string[] allowedExtensions)
        {
            int categoryCount = source.Files.Count(file => file.PublishMeta.PublishStatus == FileStatus.Published && allowedExtensions.Contains(file.Extension));
            return new CategoryDto(source.UID, source.CategoryName, source.CategoryDescription, categoryCount, ParentName: source.ParentCategory?.CategoryName);
        }
    }
}
