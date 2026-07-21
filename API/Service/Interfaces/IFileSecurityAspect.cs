using MyLife.Domain.Entities;
using MyLife.Service.Search.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Interfaces
{
    public interface IFileSecurityAspect
    {
        Task<IQueryable<FileMetaEntity>> ApplyAccessPolicy(
            IQueryable<FileMetaEntity> queryable,
            FileAccessMode mode,
            bool isAuthenticated);
    }
}
