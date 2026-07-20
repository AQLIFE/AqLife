# Service 层 - 业务逻辑实现层

## 📝 概述

**Service 层** 是系统的**核心业务逻辑实现层**。它连接 Application 层的 Handler 和 Data 层的数据访问，实现具体的业务操作。

### 职责
- 🧮 实现业务逻辑计算
- 🔄 数据映射（Entity ↔ DTO）
- 🔐 认证与授权逻辑
- 🔍 搜索和过滤功能
- 📊 数据聚合和转换

### 特点
- ✅ **无状态** - 线程安全，便于并发
- ✅ **可测试** - 逻辑清晰，易于 Mock
- ✅ **可重用** - 被 Handler 调用，也可被其他服务调用

---

## 🏗️ 目录结构

```
Service/
├── EntityService/                # 实体服务（业务逻辑核心）
│   ├── AccountService.cs         # 账户业务
│   ├── FileService.cs            # 文件业务
│   ├── CorpusService.cs          # 语料库业务
│   ├── TagServices.cs            # 标签业务
│   ├── TodoServices.cs           # 待办业务
│   └── SearchSpecification.cs    # 搜索规范
├── Features/                      # 功能服务
│   └── AuthService.cs            # JWT 认证
├── Interfaces/                    # 接口定义
│   ├── IJwtProvider.cs           # JWT 提供者接口
│   ├── IMapper.cs                # 映射接口
│   └── IStrategy/
│       └── ISearchStrategy.cs    # 搜索策略接口
├── Mapper/                        # DTO 映射器
│   ├── AccoutMapper.cs           # 账户映射
│   ├── FileMapper.cs             # 文件映射
│   ├── TagMapper.cs              # 标签映射
│   └── TodoMapper.cs             # 待办映射
├── Repository/                    # 数据访问（推荐改进）
│   └── ...新增区域...
├── Search/                        # 搜索实现
│   └── File/
│       ├── FileSearch.cs         # 搜索协调器
│       └── Strategy/
│           ├── AllFilesSearchStrategy.cs
│           └── FilteredFilesSearchStrategy.cs
├── Service.cs                     # DI 配置
└── Service.csproj
```

---

## 🔑 核心组件解读

### 1. EntityService - 业务逻辑实现

#### 设计原则

```csharp
namespace MyLife.Service.EntityService
{
	/// <summary>
	/// 账户业务服务 - 处理所有账户相关的业务逻辑
	/// </summary>
	public class AccountService
	{
		private readonly AppStorage _context;      // 数据库访问
		private readonly IMapper _mapper;          // DTO 映射
		private readonly IValidator _validator;    // 验证器

		public AccountService(AppStorage context, IMapper mapper, IValidator validator)
		{
			_context = context;
			_mapper = mapper;
			_validator = validator;
		}

		// 业务方法 - 由 Handler 调用
		public async Task<Guid> CreateAsync(CreateAccountCommand cmd)
		{
			// 1. 验证（通常在 Application 层完成，这里可选）
			// 2. 业务逻辑
			// 3. 数据库操作
			// 4. 返回结果

			var entity = new AccountEntity
			{
				Email = cmd.Email,
				PasswordHash = cmd.PasswordHash
			};

			await _context.Accounts.AddAsync(entity);
			await _context.SaveChangesAsync();

			return entity.Id;
		}

		public async Task<AccountDto> GetAsync(Guid id)
		{
			var entity = await _context.Accounts.FindAsync(id);
			if (entity == null) throw new EntityNotFoundException(nameof(Account), id);

			return _mapper.Map(entity);
		}

		public async Task UpdateAsync(Guid id, UpdateAccountCommand cmd)
		{
			var entity = await _context.Accounts.FindAsync(id);
			if (entity == null) throw new EntityNotFoundException(nameof(Account), id);

			// 更新属性
			entity.Email = cmd.Email;
			entity.DisplayName = cmd.DisplayName;

			_context.Accounts.Update(entity);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(Guid id)
		{
			var entity = await _context.Accounts.FindAsync(id);
			if (entity == null) throw new EntityNotFoundException(nameof(Account), id);

			_context.Accounts.Remove(entity);
			await _context.SaveChangesAsync();
		}
	}
}
```

#### 关键方法命名约定

```
Create{Entity}Async    - 创建实体
Get{Entity}Async       - 获取单个实体
Update{Entity}Async    - 更新实体
Delete{Entity}Async    - 删除实体
Query{Entities}Async   - 查询多个实体
Search{Entities}Async  - 搜索实体
```

---

### 2. Mapper - DTO 转换

#### 设计模式

```csharp
namespace MyLife.Service.Mapper
{
	/// <summary>
	/// 账户映射器 - 将 Entity 转换为 DTO，隐藏内部实现
	/// </summary>
	public class AccountMapper : IMapper
	{
		public AccountDto Map(AccountEntity entity)
		{
			if (entity == null) return null;

			return new AccountDto
			{
				Id = entity.Id,
				Email = entity.Email,
				DisplayName = entity.DisplayName,
				CreatedAt = entity.CreatedAt,
				// 不暴露 PasswordHash 等敏感信息
			};
		}

		// 批量映射
		public IEnumerable<AccountDto> MapList(IEnumerable<AccountEntity> entities)
		{
			return entities?.Select(Map);
		}

		// 反向映射（DTO → Entity，通常用于创建/更新）
		public AccountEntity Map(CreateAccountCommand cmd)
		{
			return new AccountEntity
			{
				Email = cmd.Email,
				// PasswordHash 应该在 Service 中进行哈希处理
			};
		}
	}
}
```

#### 映射的职责

```csharp
// ✅ 映射器应该做的事
- Entity → DTO（隐藏敏感字段）
- DTO → Entity（用于创建）
- 字段重命名
- 数据类型转换
- 嵌套对象映射

// ❌ 映射器不应该做的事
- 业务逻辑（如验证、计算）
- 数据库查询
- 事务管理
- 异常处理
```

---

### 3. 搜索与策略模式

#### 策略接口定义

```csharp
namespace MyLife.Service.Interfaces.IStrategy
{
	/// <summary>
	/// 搜索策略接口 - 定义搜索行为的约定
	/// </summary>
	public interface ISearchStrategy
	{
		// 各策略自定义方法
	}
}
```

#### 具体策略实现

```csharp
namespace MyLife.Service.Search.File.Strategy
{
	/// <summary>
	/// 查询所有文件的策略
	/// </summary>
	public class AllFilesSearchStrategy : ISearchStrategy
	{
		private readonly AppStorage _context;

		public AllFilesSearchStrategy(AppStorage context)
		{
			_context = context;
		}

		public async Task<IEnumerable<FileDto>> Execute(Guid userId)
		{
			var entities = await _context.FileMetaEntities
				.Where(f => f.UserId == userId)
				.ToListAsync();

			return entities.Select(Map);
		}
	}

	/// <summary>
	/// 按条件过滤查询文件的策略
	/// </summary>
	public class FilteredFilesSearchStrategy : ISearchStrategy
	{
		private readonly AppStorage _context;

		public FilteredFilesSearchStrategy(AppStorage context)
		{
			_context = context;
		}

		public async Task<IEnumerable<FileDto>> Execute(
			Guid userId, 
			string searchTerm, 
			string fileType)
		{
			var query = _context.FileMetaEntities
				.Where(f => f.UserId == userId);

			if (!string.IsNullOrEmpty(searchTerm))
				query = query.Where(f => f.FileName.Contains(searchTerm));

			if (!string.IsNullOrEmpty(fileType))
				query = query.Where(f => f.FileType == fileType);

			var entities = await query.ToListAsync();
			return entities.Select(Map);
		}
	}
}
```

#### 搜索协调器

```csharp
namespace MyLife.Service.Search.File
{
	/// <summary>
	/// 文件搜索协调器 - 选择合适的策略并执行
	/// </summary>
	public class FileSearch
	{
		private readonly AppStorage _context;

		public FileSearch(AppStorage context)
		{
			_context = context;
		}

		/// <summary>
		/// 执行搜索 - 根据条件选择策略
		/// </summary>
		public async Task<IEnumerable<FileDto>> ExecuteAsync(
			Guid userId,
			SearchSpecification spec)
		{
			ISearchStrategy strategy = spec.HasFilters()
				? new FilteredFilesSearchStrategy(_context)
				: new AllFilesSearchStrategy(_context);

			return await strategy.Execute(userId, spec);
		}
	}
}
```

#### 优势分析

```
✅ 易于扩展：添加新的搜索方式只需新建策略类
✅ 易于测试：每个策略可独立测试
✅ 易于维护：逻辑清晰，没有 if-else 地狱
✅ 易于复用：策略可在多个地方使用
```

---

### 4. 认证服务 - JWT 提供者

```csharp
namespace MyLife.Service.Features
{
	/// <summary>
	/// 认证服务 - JWT Token 生成和验证
	/// </summary>
	public class AuthService : IJwtProvider<AccountEntity>
	{
		private readonly JwtOption _jwtOption;

		public AuthService(IOptions<JwtOption> options)
		{
			_jwtOption = options.Value;
		}

		/// <summary>
		/// 生成 JWT Token
		/// </summary>
		public string GenerateToken(AccountEntity user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_jwtOption.SecretKey);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
					new Claim(ClaimTypes.Email, user.Email),
					new Claim("DisplayName", user.DisplayName ?? "")
				}),
				Expires = DateTime.UtcNow.AddHours(_jwtOption.ExpirationHours),
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(key), 
					SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		/// <summary>
		/// 验证和解析 Token
		/// </summary>
		public ClaimsPrincipal ValidateToken(string token)
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var key = Encoding.ASCII.GetBytes(_jwtOption.SecretKey);

				var principal = tokenHandler.ValidateToken(token, 
					new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(key),
						ValidateIssuer = false,
						ValidateAudience = false
					}, 
					out var validatedToken);

				return principal;
			}
			catch
			{
				return null;
			}
		}
	}
}
```

---

## 🔄 数据流示例

### 创建账户的完整流程

```
1. Controller 接收请求
   POST /api/accounts
   {
	 "email": "user@example.com",
	 "password": "password123"
   }
   ↓

2. Controller 创建命令对象
   var cmd = new CreateAccountCommand 
   { 
	 Email = "user@example.com",
	 PasswordHash = PasswordHasher.Hash("password123")
   }
   ↓

3. 通过 MediatR 分发
   await _mediator.Send(cmd);
   ↓

4. ValidationBehavior 验证命令
   - 检查 Email 格式
   - 检查 Email 唯一性
   - 验证通过则继续
   ↓

5. TransactionBehavior 开启事务
   using var transaction = _context.Database.BeginTransaction();
   ↓

6. CreateAccountHandler 处理
   public async Task<Guid> Handle(CreateAccountCommand request, ...)
   {
	 return await _accountService.CreateAsync(request);
   }
   ↓

7. AccountService 执行业务逻辑
   - 创建 AccountEntity 对象
   - 保存到数据库
   - 返回 Entity.Id (Guid)
   ↓

8. TransactionBehavior 提交事务
   await transaction.CommitAsync();
   ↓

9. Handler 返回结果
   return accountId;
   ↓

10. Controller 返回 HTTP 响应
	201 Created
	Location: /api/accounts/{accountId}
	Body: { "id": "..." }
```

---

## 💡 设计最佳实践

### 1️⃣ Service 应该是无状态的

```csharp
// ✅ 好的做法 - 无状态
public class AccountService
{
	private readonly AppStorage _context;

	public AccountService(AppStorage context)
	{
		_context = context;  // 依赖注入
	}
}

// ❌ 不好的做法 - 有状态
public class AccountService
{
	private AppStorage _context;  // 私有字段可能导致状态问题
	private List<Account> _cache;  // 缓存在内存中（多线程不安全）
}
```

### 2️⃣ 业务逻辑应该在 Service 中，不在 Entity 中

```csharp
// ❌ 不好的做法 - Entity 中有逻辑
public class AccountEntity
{
	public string Email { get; set; }
	public void ChangePassword(string newPassword)  // ❌ 业务逻辑不应在 Entity
	{
		PasswordHash = HashPassword(newPassword);
	}
}

// ✅ 好的做法 - Service 中实现逻辑
public class AccountService
{
	public async Task ChangePasswordAsync(Guid accountId, string newPassword)
	{
		var entity = await _context.Accounts.FindAsync(accountId);
		entity.PasswordHash = HashPassword(newPassword);
		await _context.SaveChangesAsync();
	}
}
```

### 3️⃣ 充分利用异步操作

```csharp
// ✅ 推荐 - 异步方法
public async Task<AccountDto> GetAsync(Guid id)
{
	var entity = await _context.Accounts.FindAsync(id);
	return _mapper.Map(entity);
}

// ❌ 避免 - 同步方法阻塞线程
public AccountDto Get(Guid id)
{
	var entity = _context.Accounts.Find(id);
	return _mapper.Map(entity);
}
```

### 4️⃣ 异常处理应该在 Service 中进行

```csharp
// ✅ 好的做法 - Service 处理异常
public async Task<AccountDto> GetAsync(Guid id)
{
	var entity = await _context.Accounts.FindAsync(id);
	if (entity == null)
		throw new EntityNotFoundException(nameof(Account), id);

	return _mapper.Map(entity);
}

// ❌ 不好的做法 - 让 null 传播
public async Task<AccountDto> GetAsync(Guid id)
{
	var entity = await _context.Accounts.FindAsync(id);
	return _mapper.Map(entity);  // 如果 entity 是 null 会导致异常
}
```

---

## 🧪 单元测试示例

```csharp
[TestClass]
public class AccountServiceTests
{
	private readonly Mock<AppStorage> _mockContext;
	private readonly Mock<IMapper> _mockMapper;
	private readonly AccountService _service;

	public AccountServiceTests()
	{
		_mockContext = new Mock<AppStorage>();
		_mockMapper = new Mock<IMapper>();
		_service = new AccountService(_mockContext.Object, _mockMapper.Object);
	}

	[TestMethod]
	public async Task CreateAsync_ShouldReturnAccountId()
	{
		// Arrange
		var cmd = new CreateAccountCommand
		{
			Email = "test@example.com",
			PasswordHash = "hashed"
		};
		var expectedId = Guid.NewGuid();

		// Act
		var result = await _service.CreateAsync(cmd);

		// Assert
		Assert.AreEqual(expectedId, result);
	}

	[TestMethod]
	public async Task GetAsync_WhenAccountNotFound_ShouldThrowException()
	{
		// Arrange
		var id = Guid.NewGuid();
		_mockContext.Setup(c => c.Accounts.FindAsync(id))
			.ReturnsAsync((AccountEntity)null);

		// Act & Assert
		await Assert.ThrowsExceptionAsync<EntityNotFoundException>(
			() => _service.GetAsync(id));
	}
}
```

---

## 🚀 性能优化建议

1. **数据库查询优化**
   ```csharp
   // 使用 Include 避免 N+1 查询
   var accounts = await _context.Accounts
	   .Include(a => a.Files)
	   .ToListAsync();
   ```

2. **缓存常用数据**
   ```csharp
   // 缓存用户偏好设置
   var cached = await _cache.GetAsync<UserPreferences>(cacheKey);
   ```

3. **批量操作**
   ```csharp
   // 批量插入而不是逐个插入
   await _context.Accounts.AddRangeAsync(accounts);
   ```

---

## 📖 相关文档

- [README.md](../README.md) - 项目总体说明
- [Domain README](../Domain/README.md) - 命令和查询定义
- [PROJECT_STRUCTURE.md](../PROJECT_STRUCTURE.md) - 项目结构

---

**版本：** 1.0  
**最后更新：** 2026年5月
