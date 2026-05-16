using Microsoft.AspNetCore.Http;
using MyLife.Data.Entities;
using MyLife.Shared.Options;

namespace MyLife.Service.Interfaces
{
    public interface IFileSearchStrategy
    {
        // 判断当前参数是否适用于该策略
        bool IsMatch(string? title, Guid? id);
        // 执行查询逻辑
        Task<FileMetaEntity?> ExecuteAsync(IQueryable<FileMetaEntity> query, string? title, Guid? id);
    }

    public interface IUploadCheckStrategy
    {
        // 执行校验逻辑
        (bool IsValid, string Message) Check(IFormFile file, FileOption policy);
    }

    public interface IUploadCheckStrategyAsync
    {
        // 执行异步校验逻辑
        Task<(bool IsValid, string Message)> CheckAsync(IFormFile file, FileOption policy);
    }
}
