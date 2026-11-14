using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.Contracts.CommandInterface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AqLife.Application.Business.Category.Validator
{
    /// <summary>
    /// 父分类必须存在或者等于null
    /// </summary>
    /// <param name="dbContext"></param>
    public sealed class UpdateCategoryValidator(IApplicationDbContext dbContext)
    : AbstractValidator<UpdateCategoryCommand>
    {
        private protected override string ErrorMessage { init; get; } = "父分类不存在";

        private protected override async Task<bool> IsValidAsync(UpdateCategoryCommand command, CancellationToken ct)
            => command.ParentId == null || await dbContext.Categories.AnyAsync(category => category.UID == command.ParentId, ct);
    }// 允许父分类为null

    /// <summary>
    /// 不允许更新 默认分类
    /// </summary>
    public sealed class UpdateDefaultCategoryValidator : AbstractValidator<UpdateCategoryCommand>
    {
        private protected override string ErrorMessage { init; get; } = "默认分类不允许创建父分类";

        private protected override async Task<bool> IsValidAsync(UpdateCategoryCommand command, CancellationToken ct)
            => command.UID != CategoryEntity.DefaultCategoryId;
    }

    /// <summary>
    /// 重名校验
    /// </summary>
    /// <param name="dbContext"></param>
    public sealed class UpdateCategoryRepeatNameValidator(IApplicationDbContext dbContext) : AbstractValidator<UpdateCategoryCommand>
    {
        private protected override string ErrorMessage { init; get; } = "不允许分类重名";

        private protected override async Task<bool> IsValidAsync(UpdateCategoryCommand command, CancellationToken ct)
            => await dbContext.Categories.AllAsync(e => e.CategoryName != command.Name, ct);
    }

    /// <summary>
    /// 分类循环依赖验证器: 默认 command.UID 和 command.ParentId 都是存在的; 该验证器间接实现  command.UID 和 command.ParentId 不可相同的验证逻辑
    /// </summary>
    /// <param name="dbContext"></param>
    public sealed class UpdateCategoryCircularDependencyValidator(IApplicationDbContext dbContext)
   : AbstractValidator<UpdateCategoryCommand>
    {
        private protected override string ErrorMessage { init; get; } = "分类依赖存在循环";

        private protected override async Task<bool> IsValidAsync(UpdateCategoryCommand command, CancellationToken ct)
        {
            if (command.ParentId is not Guid parentId)
                return true;// 父分类可以不设置

            if (parentId == command.UID)
                return false;// 两者不可相同

            var parentMap = await dbContext.Categories
                .AsNoTracking()
                .Select(category => new { category.UID, category.ParentId })
                .ToDictionaryAsync(category => category.UID, category => category.ParentId, ct);// 得到字典

            if (!parentMap.ContainsKey(parentId))
                return false;//字典中不存在 该分类 ; 也可以视为数据库中不存在该分类

            // Walk upward from the proposed parent; reaching this category would create a cycle.
            Guid? current = parentId;
            var visited = new HashSet<Guid>();// 已检查的GUID
            while (current is Guid currentId && visited.Add(currentId))
            {
                if (currentId == command.UID)
                    return false;//两者不可相同

                current = parentMap.GetValueOrDefault(currentId);
                //   |a|b|
                //    b-->c
                //    c-->d 直到检查到 d == a
            }

            return true;
        }

    }
}
