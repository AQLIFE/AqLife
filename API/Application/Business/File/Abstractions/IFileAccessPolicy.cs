using MyLife.Domain.Entities;

namespace MyLife.Application.Business.File.Abstractions
{
    public interface IFileAccessPolicy
    {
        Task<IQueryable<FileMetaEntity>> ApplyAccessPolicy(
            IQueryable<FileMetaEntity> queryable,
            FileAccessMode mode,
            bool isAuthenticated);
    }
}
