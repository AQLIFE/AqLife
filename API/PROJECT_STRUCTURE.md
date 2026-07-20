# AqLife API - 项目结构优化建议与改进计划

## 📋 目录

1. [当前结构评估](#当前结构评估)
2. [推荐的改进方案](#推荐的改进方案)
3. [新增项目层级建议](#新增项目层级建议)
4. [命名规范统一](#命名规范统一)
5. [模块化路线图](#模块化路线图)

---

## 当前结构评估

### 现有项目层级：

```
✓ Web.csproj          (HTTP 入口层)
✓ Application.csproj  (CQRS + 管道层)
✓ Domain.csproj       (命令/查询定义层)
✓ Service.csproj      (业务逻辑层)
✓ Data.csproj         (数据访问层)
✓ Shared.csproj       (共享契约层)
```

### 现有目录结构质量评分：

| 项目 | 内聚度 | 耦合度 | 可测试性 | 总体评分 |
|------|--------|--------|----------|---------|
| Web | 🟢 优 | 🟢 低 | 🟡 中 | 8.5/10 |
| Application | 🟢 优 | 🟢 低 | 🟢 优 | 9/10 |
| Domain | 🟢 优 | 🟢 低 | 🟢 优 | 9.5/10 |
| Service | 🟡 中 | 🟡 中 | 🟡 中 | 7/10 |
| Data | 🟢 优 | 🟡 中 | 🟡 中 | 8/10 |
| Shared | 🟢 优 | 🟢 低 | 🟢 优 | 9/10 |

---

## 推荐的改进方案

### 1. **Service 层的接口隔离改进**

#### 当前问题：
```csharp
// ❌ 缺少接口定义，不利于单元测试和 Mock
public class AccountService 
{
	// 实现代码
}
```

#### 改进方案：
```csharp
// ✅ 为每个 EntityService 定义接口
namespace MyLife.Service.Interfaces.EntityServices
{
	public interface IAccountService
	{
		Task<AccountDto> GetAsync(Guid id);
		Task<AccountDto> CreateAsync(CreateAccountCommand cmd);
		Task UpdateAsync(Guid id, UpdateAccountCommand cmd);
		Task DeleteAsync(Guid id);
	}

	public interface IFileService
	{
		Task<FileDto> GetMetadataAsync(Guid fileId);
		Task<FileDto> CreateAsync(CreateFileCommand cmd);
		Task DeleteAsync(Guid fileId);
		Task<Stream> GetFileStreamAsync(Guid fileId);
	}

	// 其他 Service 接口...
}
```

#### 实现步骤：
1. 在 `Service/Interfaces/EntityServices/` 下创建接口文件
2. 为每个 EntityService 提取接口
3. 更新 DI 容器配置（Service.cs）
4. 更新 Handler 中的依赖注入

---

### 2. **Repository 模式的完善**

#### 当前状态：
- 直接使用 `AppStorage` DbContext
- 缺少通用仓储接口

#### 改进方案：
```csharp
// Service/Repository/IRepository.cs
namespace MyLife.Service.Interfaces.Repository
{
	public interface IRepository<T> where T : class
	{
		Task<T> GetByIdAsync(Guid id);
		Task<IEnumerable<T>> GetAllAsync();
		Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
		Task<T> AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task DeleteAsync(T entity);
		Task SaveChangesAsync();
	}
}

// Service/Repository/GenericRepository.cs
public class GenericRepository<T> : IRepository<T> where T : class
{
	private readonly AppStorage _context;
	private readonly DbSet<T> _dbSet;

	public GenericRepository(AppStorage context)
	{
		_context = context;
		_dbSet = context.Set<T>();
	}

	public async Task<T> GetByIdAsync(Guid id)
	{
		return await _dbSet.FindAsync(id);
	}

	// 其他实现...
}

// Service/Repository/IUnitOfWork.cs
public interface IUnitOfWork
{
	IRepository<AccountEntity> Accounts { get; }
	IRepository<FileMetaEntity> Files { get; }
	IRepository<TagEntity> Tags { get; }
	// 其他仓储...

	Task SaveChangesAsync();
	Task BeginTransactionAsync();
	Task CommitAsync();
	Task RollbackAsync();
}
```

#### 优势：
- 统一数据访问接口
- 便于单元测试（可 Mock）
- 减少重复代码

---

### 3. **业务异常体系的完善**

#### 当前异常类型：
```
BusinessException
DatabaseException
FileException
OptionException
RequestException
```

#### 改进方案：
```csharp
namespace MyLife.Shared.Exceptions
{
	// 基础异常
	public abstract class AppException : Exception
	{
		public int ErrorCode { get; set; }
		public string ErrorType { get; set; }

		protected AppException(string message, int errorCode = 500, 
							  string errorType = "InternalServerError") 
			: base(message)
		{
			ErrorCode = errorCode;
			ErrorType = errorType;
		}
	}

	// 业务异常 (422)
	public class BusinessException : AppException
	{
		public BusinessException(string message) 
			: base(message, 422, "UnprocessableEntity") { }
	}

	// 验证异常 (400)
	public class ValidationException : AppException
	{
		public Dictionary<string, string> Errors { get; set; }

		public ValidationException(Dictionary<string, string> errors) 
			: base("Validation failed", 400, "ValidationError")
		{
			Errors = errors;
		}
	}

	// 未找到异常 (404)
	public class EntityNotFoundException : AppException
	{
		public EntityNotFoundException(string entityName, Guid id) 
			: base($"{entityName} with id {id} not found", 404, "NotFound") { }
	}

	// 未授权异常 (401)
	public class UnauthorizedException : AppException
	{
		public UnauthorizedException(string message = "Unauthorized") 
			: base(message, 401, "Unauthorized") { }
	}

	// 禁止访问异常 (403)
	public class ForbiddenException : AppException
	{
		public ForbiddenException(string message = "Forbidden") 
			: base(message, 403, "Forbidden") { }
	}

	// 数据库异常 (500)
	public class DataBaseException : AppException
	{
		public DataBaseException(string message, Exception innerException = null) 
			: base(message, 500, "DatabaseError") { }
	}
}
```

---

### 4. **全局响应格式统一**

#### 当前问题：
- 异常处理中间件格式可能不统一
- API 响应格式可能不一致

#### 改进方案：
```csharp
// Shared/Common/ApiResponse.cs
public class ApiResponse<T>
{
	public bool Success { get; set; }
	public T Data { get; set; }
	public string Message { get; set; }
	public ApiError Error { get; set; }
	public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}

public class ApiError
{
	public string Code { get; set; }
	public string Type { get; set; }
	public string Message { get; set; }
	public Dictionary<string, string> Details { get; set; }
}

// Web/Middlewares/ApiResponseMiddleware.cs
public class ApiResponseMiddleware
{
	public async Task InvokeAsync(HttpContext context, ILogger<ApiResponseMiddleware> logger)
	{
		var response = context.Response;
		response.ContentType = "application/json";

		try
		{
			// ... 执行管道
		}
		catch (AppException ex)
		{
			response.StatusCode = ex.ErrorCode;
			var apiResponse = new ApiResponse<object>
			{
				Success = false,
				Message = ex.Message,
				Error = new ApiError 
				{ 
					Code = ex.ErrorCode.ToString(),
					Type = ex.ErrorType,
					Message = ex.Message
				}
			};
			await response.WriteAsJsonAsync(apiResponse);
		}
	}
}
```

---

## 新增项目层级建议

### 建议的扩展结构：

```
API/
├── Web/                        (HTTP 入口)
├── Application/                (CQRS + 管道)
├── Domain/                     (命令/查询定义)
├── Service/                    (业务逻辑)
├── Data/                       (数据访问)
├── Shared/                     (共享契约)
│
├── 🆕 Infrastructure/          (基础设施支持)
│   ├── Cache/                      # 缓存实现（Redis）
│   ├── BackgroundJobs/             # 后台任务（Hangfire）
│   ├── Notifications/              # 通知服务（Email、SMS）
│   └── ExternalAPIs/               # 第三方 API 集成
│
├── 🆕 Tests/                   (测试项目组)
│   ├── UnitTests/                  # 单元测试（xUnit）
│   ├── IntegrationTests/           # 集成测试
│   └── PerformanceTests/           # 性能测试
│
└── 🆕 Specifications/          (API 规范)
	├── OpenAPI/                    # OpenAPI 规范文件
	└── Examples/                   # 示例数据与场景
```

### 各新增层的职责：

#### Infrastructure 层
- **缓存管理** - Redis 缓存、本地缓存策略
- **后台任务** - 定时任务、队列处理
- **通知服务** - Email、SMS、推送通知
- **外部集成** - 第三方 API 调用

#### Tests 层
- **单元测试** - 验证单个组件的逻辑正确性
- **集成测试** - 验证多层组件协作
- **性能测试** - 基准测试和压力测试

#### Specifications 层
- **API 文档** - 自动生成的 OpenAPI 规范
- **示例数据** - 各个场景的数据示例

---

## 命名规范统一

### 当前存在的问题：

| 问题 | 现状 | 改进 |
|------|------|------|
| 文件命名 | `DataLayerSetup .cs` (有空格) | `DataLayerSetup.cs` |
| 方法命名 | `DowanloadFileHandler` | `DownloadFileHandler` |
| 查询处理 | `TagQueryHandler` (语义不清) | `QueryTagsHandler` |
| 类命名 | 混用单数复数 | 统一使用复数形式（如 `TagServices`） |

### 建议的命名规范：

```csharp
// ✅ 命令处理器命名
public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid> { }
public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, Guid> { }
public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Guid> { }

// ✅ 查询处理器命名
public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountDto> { }
public class QueryAccountsQueryHandler : IRequestHandler<QueryAccountsQuery, IEnumerable<AccountDto>> { }

// ✅ 服务接口命名
public interface IAccountService { }
public interface IFileService { }
public interface ITagService { }

// ✅ 服务实现命名
public class AccountService : IAccountService { }
public class FileService : IFileService { }

// ✅ 验证器命名
public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand> { }
public class UpdateFileCommandValidator : AbstractValidator<UpdateFileCommand> { }

// ✅ Mapper 命名
public class AccountMapper : IMapper<AccountEntity, AccountDto> { }
public class FileMapper : IMapper<FileMetaEntity, FileDto> { }

// ✅ 策略命名
public class AllFilesSearchStrategy : ISearchStrategy { }
public class FilteredFilesSearchStrategy : ISearchStrategy { }

// ✅ Repository 命名
public class GenericRepository<T> : IRepository<T> { }
public class UnitOfWork : IUnitOfWork { }
```

---

## 模块化路线图

### 第一阶段（月度 1）- 基础优化
```
[ ] 1. 为所有 EntityService 定义接口
[ ] 2. 统一命名（修复拼写错误、空格等）
[ ] 3. 完善异常体系，统一错误代码
[ ] 4. 增强 Swagger 文档注解
```

### 第二阶段（月度 2）- 数据访问层重构
```
[ ] 1. 实现通用 Repository 模式
[ ] 2. 实现 UnitOfWork 模式
[ ] 3. 创建 Repository 接口
[ ] 4. 迁移所有数据访问代码
[ ] 5. 编写单元测试
```

### 第三阶段（月度 3）- 测试框架建设
```
[ ] 1. 创建 UnitTests 项目（xUnit + Moq）
[ ] 2. 创建 IntegrationTests 项目
[ ] 3. 编写核心业务逻辑的测试用例
[ ] 4. 设置 CI/CD 流水线
```

### 第四阶段（月度 4）- 基础设施扩展
```
[ ] 1. 集成 Redis 缓存
[ ] 2. 集成 Hangfire 后台任务
[ ] 3. 实现分布式缓存策略
[ ] 4. 性能基准测试
```

### 第五阶段（月度 5+）- 企业级功能
```
[ ] 1. 多租户支持
[ ] 2. 审计日志系统
[ ] 3. API 网关（Kong/Ocelot）
[ ] 4. 分布式追踪（Jaeger）
[ ] 5. 监控告警（Prometheus + Grafana）
```

---

## 项目文件结构建议

### 完整的改进后结构：

```
AqLife/
├── API/
│   ├── Web/
│   │   ├── Controllers/
│   │   │   ├── AccountController.cs
│   │   │   ├── BlogController.cs
│   │   │   ├── CorpusController.cs
│   │   │   ├── FileController.cs
│   │   │   ├── TagController.cs
│   │   │   └── TodoController.cs
│   │   ├── Extensions/
│   │   │   ├── ExceptionHandlerSetup.cs
│   │   │   ├── FilePolicy.cs
│   │   │   ├── InfrastructureSetup.cs
│   │   │   ├── JwtSetup.cs
│   │   │   └── RoutingSetup.cs
│   │   ├── Middlewares/
│   │   │   ├── ExceptionHandler.cs
│   │   │   ├── FilePolicyFilter.cs
│   │   │   └── ApiResponseMiddleware.cs  # 新增
│   │   ├── Configurations/
│   │   │   ├── appsettings.json
│   │   │   ├── appsettings.Development.json
│   │   │   ├── appsettings.Production.json
│   │   │   ├── FilePolicy.json
│   │   │   └── FilePolicy.*.json
│   │   ├── Program.cs
│   │   ├── Web.csproj
│   │   └── Web.http
│   │
│   ├── Application/
│   │   ├── Handlers/
│   │   │   ├── Account/
│   │   │   ├── File/
│   │   │   ├── Tag/
│   │   │   └── Todo/
│   │   ├── Behaviors/
│   │   │   ├── ValidationBehavior.cs
│   │   │   └── TransactionBehavior.cs
│   │   ├── Validators/
│   │   │   ├── AbstractValidator.cs
│   │   │   ├── ExistenceValidator.cs
│   │   │   └── BusinessValidator/
│   │   │       ├── AccountValidator.cs
│   │   │       ├── FileValidator.cs
│   │   │       ├── TagValidator.cs
│   │   │       └── TodoValidator.cs
│   │   ├── Application.cs
│   │   └── Application.csproj
│   │
│   ├── Domain/
│   │   ├── CommandInterface/
│   │   │   ├── CoreFoundation.cs
│   │   │   ├── AspectMarkerInterfaces.cs
│   │   │   └── SemanticCommand.cs
│   │   ├── Command/
│   │   │   ├── AccountCommand.cs
│   │   │   ├── FileCommand.cs
│   │   │   ├── TagCommand.cs
│   │   │   └── TodoCommand.cs
│   │   ├── Domain.csproj
│   │   └── README.md  # 新增
│   │
│   ├── Service/
│   │   ├── EntityService/
│   │   │   ├── AccountService.cs
│   │   │   ├── FileService.cs
│   │   │   ├── CorpusService.cs
│   │   │   ├── TagServices.cs
│   │   │   ├── TodoServices.cs
│   │   │   └── SearchSpecification.cs
│   │   ├── Features/
│   │   │   └── AuthService.cs
│   │   ├── Interfaces/
│   │   │   ├── IJwtProvider.cs
│   │   │   ├── IMapper.cs
│   │   │   ├── IStrategy/
│   │   │   │   └── ISearchStrategy.cs
│   │   │   └── EntityServices/  # 新增
│   │   │       ├── IAccountService.cs
│   │   │       ├── IFileService.cs
│   │   │       ├── ITagService.cs
│   │   │       └── ITodoService.cs
│   │   ├── Mapper/
│   │   │   ├── AccoutMapper.cs
│   │   │   ├── FileMapper.cs
│   │   │   ├── TagMapper.cs
│   │   │   └── TodoMapper.cs
│   │   ├── Repository/  # 新增
│   │   │   ├── IRepository.cs
│   │   │   ├── GenericRepository.cs
│   │   │   ├── IUnitOfWork.cs
│   │   │   └── UnitOfWork.cs
│   │   ├── Search/
│   │   │   └── File/
│   │   │       ├── FileSearch.cs
│   │   │       └── Strategy/
│   │   │           ├── AllFilesSearchStrategy.cs
│   │   │           └── FilteredFilesSearchStrategy.cs
│   │   ├── Service.cs
│   │   ├── Service.csproj
│   │   └── README.md  # 新增
│   │
│   ├── Data/
│   │   ├── Entities/
│   │   │   ├── AccountEntity.cs
│   │   │   ├── CorpusEntity.cs
│   │   │   ├── FileMetaEntity.cs
│   │   │   ├── FileTagEntity.cs
│   │   │   ├── TagEntity.cs
│   │   │   └── TodoEntity.cs
│   │   ├── Repository/
│   │   │   └── AppStorage.cs
│   │   ├── Migrations/
│   │   │   ├── 20260506070312_InitialCreate.cs
│   │   │   ├── ... (其他迁移)
│   │   │   └── AppStorageModelSnapshot.cs
│   │   ├── DataLayerSetup.cs  # 修复文件名
│   │   ├── Data.csproj
│   │   └── README.md  # 新增
│   │
│   ├── Shared/
│   │   ├── Contracts/
│   │   │   ├── IEntity.cs
│   │   │   ├── IUserEntity.cs
│   │   │   ├── ISimpleAccountInfo.cs
│   │   │   └── IValidator.cs
│   │   ├── Exceptions/
│   │   │   ├── BusinessException.cs
│   │   │   ├── DataBaseException.cs
│   │   │   ├── FileException.cs
│   │   │   ├── OptionException.cs
│   │   │   ├── RequestException.cs
│   │   │   ├── EntityNotFoundException.cs  # 新增
│   │   │   ├── UnauthorizedException.cs  # 新增
│   │   │   ├── ForbiddenException.cs  # 新增
│   │   │   └── ValidationException.cs  # 新增
│   │   ├── Common/  # 新增
│   │   │   ├── ApiResponse.cs
│   │   │   ├── ApiError.cs
│   │   │   └── PagedResult.cs
│   │   ├── IView/
│   │   │   ├── IEntityDto.cs
│   │   │   ├── AccountDto.cs
│   │   │   ├── FileDto.cs
│   │   │   ├── TagDto.cs
│   │   │   ├── TodoDto.cs
│   │   │   └── SubscriptionDto.cs
│   │   ├── Options/
│   │   │   ├── DbOption.cs
│   │   │   ├── JwtOption.cs
│   │   │   ├── FileOption.cs
│   │   │   └── APIStatus.cs
│   │   ├── Tools/
│   │   │   ├── GetUserID.cs
│   │   │   ├── GetFiles.cs
│   │   │   └── UploadContext.cs
│   │   ├── Utils/
│   │   │   ├── FastHash.cs
│   │   │   └── IdentityGenerator.cs
│   │   ├── Shared.csproj
│   │   └── README.md  # 新增
│   │
│   ├── Infrastructure/  # 新增
│   │   ├── Cache/
│   │   │   ├── ICacheService.cs
│   │   │   ├── RedisCacheService.cs
│   │   │   └── MemoryCacheService.cs
│   │   ├── BackgroundJobs/
│   │   │   ├── IBackgroundJobService.cs
│   │   │   └── HangfireJobService.cs
│   │   ├── Notifications/
│   │   │   ├── IEmailService.cs
│   │   │   ├── ISmsService.cs
│   │   │   └── IPushNotificationService.cs
│   │   ├── ExternalAPIs/
│   │   │   ├── IPaymentGateway.cs
│   │   │   └── IOssService.cs
│   │   ├── Infrastructure.csproj
│   │   └── README.md  # 新增
│   │
│   ├── Tests/  # 新增
│   │   ├── UnitTests/
│   │   │   ├── ApplicationTests/
│   │   │   │   └── HandlersTests/
│   │   │   ├── ServiceTests/
│   │   │   ├── DataTests/
│   │   │   ├── UnitTests.csproj
│   │   │   └── README.md
│   │   │
│   │   ├── IntegrationTests/
│   │   │   ├── APITests/
│   │   │   ├── DatabaseTests/
│   │   │   ├── IntegrationTests.csproj
│   │   │   └── README.md
│   │   │
│   │   └── PerformanceTests/
│   │       ├── BenchmarkTests/
│   │       ├── LoadTests/
│   │       └── PerformanceTests.csproj
│   │
│   ├── MyLife.slnx
│   ├── README.md  # 已创建 ✅
│   ├── PROJECT_STRUCTURE.md  # 此文件
│   └── ARCHITECTURE.md  # 新增
│
└── ... (其他目录)
```

---

## 实施优先级建议

### 🔴 高优先级（立即实施）
1. 修复文件名和命名规范错误
2. 为 EntityService 定义接口
3. 完善异常体系

### 🟡 中优先级（1-2 周内）
4. 实现 Repository 和 UnitOfWork 模式
5. 增强 Swagger 文档
6. 统一 API 响应格式

### 🟢 低优先级（1 个月内）
7. 创建测试项目
8. 集成 Redis 缓存
9. 性能优化

---

## 参考资源

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [Repository Pattern](https://martinfowler.com/eaaCatalog/repository.html)
- [Unit of Work Pattern](https://martinfowler.com/eaaCatalog/unitOfWork.html)
- [MediatR Documentation](https://github.com/jbogard/MediatR)

---

**文档生成时间：** 2026年5月

**适用版本：** .NET 8+

**维护者：** 架构优化委员会
