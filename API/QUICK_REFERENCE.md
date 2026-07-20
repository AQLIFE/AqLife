# AqLife API - 快速参考（Cheat Sheet）

## 🎯 快速导航

| 场景 | 访问 | 说明 |
|------|------|------|
| 项目总体 | [README.md](README.md) | 完整的项目说明 |
| 架构设计 | [ARCHITECTURE_DECISIONS.md](ARCHITECTURE_DECISIONS.md) | 设计决策记录 |
| 结构优化 | [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) | 改进建议和路线图 |
| Domain 层 | [Domain/README.md](Domain/README.md) | 命令/查询定义方法 |
| Service 层 | [Service/README.md](Service/README.md) | 业务逻辑实现方法 |

---

## 📋 常见任务速查

### 任务 1：添加新的业务功能

#### 步骤 1：定义命令（Domain 层）
```csharp
// Domain/Command/XXXCommand.cs
public class CreateTodoCommand : ICommand
{
	public Guid UserId { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
}
```

#### 步骤 2：创建处理器（Application 层）
```csharp
// Application/Handlers/CreateTodoCommandHandler.cs
public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, Guid>
{
	private readonly ITodoService _service;

	public async Task<Guid> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
	{
		return await _service.CreateAsync(request);
	}
}
```

#### 步骤 3：实现业务逻辑（Service 层）
```csharp
// Service/EntityService/TodoServices.cs
public async Task<Guid> CreateAsync(CreateTodoCommand cmd)
{
	var entity = new TodoEntity 
	{ 
		UserId = cmd.UserId,
		Title = cmd.Title,
		Description = cmd.Description
	};

	await _context.Todos.AddAsync(entity);
	await _context.SaveChangesAsync();
	return entity.Id;
}
```

#### 步骤 4：创建控制器（Web 层）
```csharp
// Web/Controllers/TodoController.cs
[HttpPost]
public async Task<ActionResult<Guid>> Create([FromBody] CreateTodoCommand cmd)
{
	var id = await _mediator.Send(cmd);
	return CreatedAtAction(nameof(Get), new { id }, id);
}
```

---

### 任务 2：添加数据验证规则

#### 步骤 1：创建验证器（Application 层）
```csharp
// Application/Validators/BusinessValidator/TodoValidator.cs
public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
	public CreateTodoCommandValidator()
	{
		RuleFor(x => x.Title)
			.NotEmpty().WithMessage("标题不能为空")
			.MaximumLength(100).WithMessage("标题长度不能超过100");

		RuleFor(x => x.UserId)
			.NotEqual(Guid.Empty).WithMessage("用户ID无效");
	}
}
```

#### 步骤 2：自动集成（已通过 ValidationBehavior）
- 不需要额外代码，ValidationBehavior 会自动执行验证器

#### 验证执行流程
```
请求发送
  ↓
ValidationBehavior 检查
  ↓
查找对应的 Validator
  ↓
执行验证规则
  ↓
验证失败 → 抛出异常 → 异常中间件处理
验证成功 → 继续执行 Handler
```

---

### 任务 3：添加数据库表

#### 步骤 1：定义实体（Data 层）
```csharp
// Data/Entities/TodoEntity.cs
public class TodoEntity : IEntity
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public TodoStatus Status { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? DueDate { get; set; }
}
```

#### 步骤 2：添加到 DbContext
```csharp
// Data/Repository/AppStorage.cs
public DbSet<TodoEntity> Todos { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
	modelBuilder.Entity<TodoEntity>()
		.HasKey(t => t.Id);

	modelBuilder.Entity<TodoEntity>()
		.HasIndex(t => t.UserId);
}
```

#### 步骤 3：创建迁移
```bash
cd API
dotnet ef migrations add AddTodoTable --project Data --startup-project Web
dotnet ef database update --project Data --startup-project Web
```

---

### 任务 4：实现自定义搜索策略

#### 步骤 1：定义策略接口
```csharp
// Service/Interfaces/IStrategy/ITodoSearchStrategy.cs
public interface ITodoSearchStrategy
{
	Task<IEnumerable<TodoDto>> ExecuteAsync(Guid userId, TodoSearchSpec spec);
}
```

#### 步骤 2：实现具体策略
```csharp
// Service/Search/Todo/Strategy/CompletedTodosStrategy.cs
public class CompletedTodosStrategy : ITodoSearchStrategy
{
	public async Task<IEnumerable<TodoDto>> ExecuteAsync(Guid userId, TodoSearchSpec spec)
	{
		var entities = await _context.Todos
			.Where(t => t.UserId == userId && t.Status == TodoStatus.Completed)
			.OrderByDescending(t => t.CreatedAt)
			.ToListAsync();

		return entities.Select(_mapper.Map);
	}
}
```

#### 步骤 3：创建协调器
```csharp
// Service/Search/Todo/TodoSearch.cs
public class TodoSearch
{
	public async Task<IEnumerable<TodoDto>> SearchAsync(Guid userId, TodoSearchSpec spec)
	{
		ITodoSearchStrategy strategy = spec.Status switch
		{
			TodoStatus.Completed => new CompletedTodosStrategy(_context),
			TodoStatus.Pending => new PendingTodosStrategy(_context),
			_ => new AllTodosStrategy(_context)
		};

		return await strategy.ExecuteAsync(userId, spec);
	}
}
```

---

### 任务 5：修复编译错误

#### 常见错误 1：缺少 using
```csharp
// ❌ 错误
var result = await _mediator.Send(cmd);

// ✅ 解决
using MediatR;
var result = await _mediator.Send(cmd);
```

#### 常见错误 2：异步不匹配
```csharp
// ❌ 错误
public async Task<Account> GetAccount(Guid id)
{
	return _context.Accounts.Find(id);  // 同步方法
}

// ✅ 解决
public async Task<Account> GetAccount(Guid id)
{
	return await _context.Accounts.FindAsync(id);  // 异步方法
}
```

#### 常见错误 3：忘记注入依赖
```csharp
// ❌ 错误
public class MyService
{
	private readonly IRepository _repo;  // 没有初始化
}

// ✅ 解决
public class MyService
{
	private readonly IRepository _repo;

	public MyService(IRepository repo)  // 通过构造函数注入
	{
		_repo = repo;
	}
}
```

---

## 🔑 关键代码片段

### 1. 注入 Service
```csharp
public class CreateTodoHandler : IRequestHandler<CreateTodoCommand, Guid>
{
	private readonly ITodoService _service;

	public CreateTodoHandler(ITodoService service)
	{
		_service = service;
	}

	public async Task<Guid> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
	{
		return await _service.CreateAsync(request);
	}
}
```

### 2. 异常处理
```csharp
try
{
	return await _service.GetAsync(id);
}
catch (EntityNotFoundException ex)
{
	return NotFound(ex.Message);
}
catch (BusinessException ex)
{
	return BadRequest(ex.Message);
}
```

### 3. 事务处理（自动）
```csharp
// ✅ 不需要手动编写，TransactionBehavior 会自动处理
public class CreateTodoCommand : ICommand  // 实现 ICommand = IRequireTransaction
{
	// 数据...
}
```

### 4. 数据映射
```csharp
var accountDto = _mapper.Map(accountEntity);  // Entity → DTO
var accountEntity = _mapper.Map(createCmd);   // Command → Entity
```

### 5. 搜索功能
```csharp
var results = await _fileSearch.ExecuteAsync(userId, searchSpec);
```

---

## 🚀 快速启动命令

```bash
# 进入 API 目录
cd API

# 恢复 NuGet 包
dotnet restore

# 构建项目
dotnet build

# 应用数据库迁移
dotnet ef database update --project Data --startup-project Web

# 运行应用
dotnet run --project Web

# 创建新迁移
dotnet ef migrations add YourMigrationName --project Data --startup-project Web

# 删除最后一个迁移
dotnet ef migrations remove --project Data --startup-project Web

# 删除数据库
dotnet ef database drop --project Data --startup-project Web
```

---

## 📁 文件放置指南

### 我应该在哪里创建文件？

| 文件类型 | 存放位置 | 示例 |
|---------|--------|------|
| 命令 | `Domain/Command/` | `CreateAccountCommand.cs` |
| 查询 | `Domain/Command/` 或分离 | `GetAccountQuery.cs` |
| 处理器 | `Application/Handlers/{Domain}/` | `CreateAccountHandler.cs` |
| 验证器 | `Application/Validators/BusinessValidator/` | `AccountValidator.cs` |
| 服务 | `Service/EntityService/` | `AccountService.cs` |
| 映射器 | `Service/Mapper/` | `AccountMapper.cs` |
| Entity | `Data/Entities/` | `AccountEntity.cs` |
| 策略 | `Service/Search/{Feature}/Strategy/` | `AllFilesSearchStrategy.cs` |
| 控制器 | `Web/Controllers/` | `AccountController.cs` |
| 中间件 | `Web/Middlewares/` | `CustomMiddleware.cs` |

---

## 🔍 调试技巧

### 1. 查看请求流
```csharp
// 在 Handler 中添加日志
public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
{
	Logger.Information("开始创建账户: {@Command}", request);

	var result = await _service.CreateAsync(request);

	Logger.Information("账户创建成功: {AccountId}", result);
	return result;
}
```

### 2. 检查验证规则
```csharp
// 在测试中验证
var validator = new CreateAccountCommandValidator();
var result = validator.Validate(new CreateAccountCommand { /* ... */ });
Assert.True(result.IsValid);
```

### 3. 数据库查询调试
```csharp
// 启用 EF Core SQL 日志
services.AddDbContext<AppStorage>(options =>
	options.UseMySql(connectionString, version)
		.LogTo(Console.WriteLine)
);
```

---

## 💾 Git 提交规范

```
feat: 添加新功能
fix: 修复 bug
docs: 文档更改
refactor: 代码重构
test: 添加或修改测试
perf: 性能优化
chore: 依赖更新、工具配置
```

### 示例：
```bash
git commit -m "feat: 添加 Todo 业务模块

- 定义 CreateTodoCommand 命令
- 实现 TodoService 业务逻辑
- 添加 TodoValidator 验证规则
- 创建 TodoController 端点"
```

---

## 🆘 获取帮助

### 遇到问题时：

1. **编译错误** → 检查 using 语句和命名空间
2. **运行时错误** → 查看异常堆栈和日志
3. **业务错误** → 检查验证规则和业务逻辑
4. **数据库错误** → 检查迁移和连接字符串
5. **找不到类** → 检查项目引用和 DI 配置

### 有用的资源：

- [MediatR 文档](https://github.com/jbogard/MediatR)
- [EF Core 文档](https://docs.microsoft.com/zh-cn/ef/core/)
- [FluentValidation 文档](https://fluentvalidation.net/)
- [Swagger/OpenAPI](https://swagger.io/)

---

## ✅ 代码检查清单

提交代码前请检查：

- [ ] 编译通过，无警告
- [ ] 遵循命名规范
- [ ] 添加了验证规则
- [ ] 添加了异常处理
- [ ] 编写了注释或文档
- [ ] 运行了相关测试
- [ ] 没有硬编码的值
- [ ] 没有注释掉的代码
- [ ] 代码符合项目风格
- [ ] 提交信息清晰明了

---

## 🎓 学习路径

### 第 1 周：理解架构
1. 阅读 [README.md](README.md)
2. 阅读 [ARCHITECTURE_DECISIONS.md](ARCHITECTURE_DECISIONS.md)
3. 理解 6 层架构的职责

### 第 2 周：学习 CQRS
1. 阅读 [Domain/README.md](Domain/README.md)
2. 理解 Command 和 Query 的区别
3. 查看现有的 Command 实现

### 第 3 周：业务逻辑实现
1. 阅读 [Service/README.md](Service/README.md)
2. 学习如何编写 Service
3. 学习如何编写 Mapper

### 第 4 周：实践
1. 完成一个完整的功能（从命令到控制器）
2. 添加验证规则
3. 编写测试

---

**最后更新：** 2026年5月  
**版本：** 1.0
