using Microsoft.AspNetCore.Http;
using MyLife.Data;
using MyLife.Data.Entities;

namespace MyLife.Service.ServiceInterfaces.IStrategy
{
    //public interface ISearchStrategy<TSource> where TSource : IStorageEntity
    //{
    //    // 判断当前参数是否适用于该策略
    //    bool IsMatch(string? title, Guid? id);
    //    // 执行查询逻辑
    //    Task<IEnumerable<TSource?>> ExecuteAsync(IQueryable<TSource> query, string? title, Guid? id);
    //}




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
