//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Options;
//using MyLife.Data.Entities;
//using MyLife.Data.Repository;
//using MyLife.Service.Interfaces.IStrategy;
//using MyLife.Shared.Options;

//namespace MyLife.Service.Implementations
//{
//    [Obsolete]
//    public class FileSearch(AppStorage storage, IHttpContextAccessor httpContext, IOptions<FilePolicyOption> options)
//    {
//        //private bool IsValid { init; get; } = httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;

//        /// <summary>
//        /// 已做特殊处理, 若存在授权,则返回全部文件列表,反之仅博客文件
//        /// </summary>
//        /// <param name="title"></param>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public async Task<IEnumerable<FileMetaEntity?>> FindFileAsync(string? title, Guid? id, bool isDown = false)
//        {
//            var strategy = searchStrategies.FirstOrDefault(s => s.IsMatch(title, id));// 获取第一个有效规则
//            if (strategy == null) return [];

//            IQueryable<FileMetaEntity> queryable = isDown || IsValid ? storage.File.AsQueryable() : storage.File.Where(e => options.Value.AllowedDownload.Contains(e.DesensitizationName)).AsQueryable();
//            return await strategy.ExecuteAsync(queryable, title, id);
//        }


//        /// <summary>
//        /// 异步从存储检索 FileMetaEntity 列表。若 IsValid 为 true 则返回全部记录；否则仅返回经 _uploadCheck 对 DesensitizationName 验证为有效的记录。以无跟踪查询
//        /// (AsNoTracking) 获取结果。
//        /// </summary>
//        /// <remarks>查询在数据库端执行，使用 EF Core 的 ToListAsync 且结果不由 DbContext 跟踪（AsNoTracking）。</remarks>
//        /// <returns>表示异步操作的任务；任务结果为 FileMetaEntity 列表，或 null。</returns>
//        public async Task<IEnumerable<FileMetaEntity?>> FindAllAsync()
//        => IsValid ? await storage.File.AsNoTracking().ToListAsync()
//        : await storage.File.AsNoTracking().Where(e => options.Value.AllowedDownload.Contains(e.DesensitizationName)).ToListAsync();
//    }
//}
