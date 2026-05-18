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
        Task<IEnumerable<FileMetaEntity?>> ExecuteAsync(IQueryable<FileMetaEntity> query, string? title, Guid? id);
    }

    
    

    public interface IUploadStrategy
    {
        (bool IsValid, string Message) Check(IFormFile file);        
    }

    public interface IDownloadStrategy
    {
        (bool IsValid, string Message) Check(string ext);
    }

    public interface IUploadStrategyAsync
    {
        // 执行异步校验逻辑
        Task<(bool IsValid, string Message)> CheckAsync(IFormFile file);
    }
}
