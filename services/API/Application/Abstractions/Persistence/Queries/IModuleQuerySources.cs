using AqLife.Domain.Entities;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Domain.Entities.Favorite;
using AqLife.Domain.Entities.File;

namespace AqLife.Application.Abstractions.Persistence.Queries;

// Read-only query ports used by shared search infrastructure.
// Each port exposes only the entity set owned by its module.
public interface IAccountQuerySource { IQueryable<AccountEntity> Query(); }
public interface ICategoryQuerySource { IQueryable<CategoryEntity> Query(); }
public interface ICorpusQuerySource { IQueryable<CorpusEntity> Query(); }
public interface IFavoriteItemQuerySource { IQueryable<FavoriteItemEntity> Query(); }
public interface IFileMetaQuerySource { IQueryable<FileMetaEntity> Query(); }
public interface IDevelopmentPlanQuerySource { IQueryable<DevelopmentPlanEntity> Query(); }
public interface ITodoQuerySource { IQueryable<TodoEntity> Query(); }
public interface ITagQuerySource { IQueryable<TagEntity> Query(); }
