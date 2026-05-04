using Microsoft.AspNetCore.Http;
using MyLife.Data.Entities;
using MyLife.Shared.Config;

namespace MyLife.Service.Interfaces
{
    public interface IFileSearchStrategy
    {
        // 判断当前参数是否适用于该策略
        bool IsMatch(string? title, Guid? id);
        // 执行查询逻辑
        Task<FileIndexEntity?> ExecuteAsync(IQueryable<FileIndexEntity> query, string? title, Guid? id);
    }

    public interface IUploadCheckStrategy
    {
        // 执行校验逻辑
        (bool IsValid, string Message) Check(IFormFile file, FilePolicy policy);
    }
}
