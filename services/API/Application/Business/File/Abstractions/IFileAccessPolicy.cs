using AqLife.Application.Business.File.Search;
using AqLife.Domain.Entities;

namespace AqLife.Application.Business.File.Abstractions
{
    public interface IFileAccessPolicy
    {
        Task<IQueryable<FileMetaEntity>> ApplyAccessPolicy(
            IQueryable<FileMetaEntity> queryable,
            FileAccessMode mode,
            bool isAuthenticated);
    }
}
