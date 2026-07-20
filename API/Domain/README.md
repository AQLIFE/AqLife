# Domain 层 - 业务领域命令与查询定义

## 📝 概述

**Domain 层** 是系统的 CQRS 模式的**命令和查询定义层**。它定义了所有业务操作的语义，是 Application 层的数据输入。

### 职责
- 📋 定义 CQRS 的核心接口（IQuery、ICommand、IAppRequest）
- 📝 定义具体的业务命令（如 CreateAccountCommand）
- 🔍 定义具体的业务查询（如 GetAccountQuery）
- 🏷️ 定义标记接口标识特殊需求（如 IRequireTransaction）

### 特点
- ✅ **零依赖** - 不依赖其他项目（甚至不依赖 EF Core）
- ✅ **纯 DTO** - 仅包含数据类，不包含逻辑
- ✅ **语义化** - 命名直观表达业务意图

---

## 🏗️ 目录结构

```
Domain/
├── CommandInterface/
│   ├── CoreFoundation.cs          # CQRS 核心接口
│   ├── AspectMarkerInterfaces.cs  # 标记接口
│   └── SemanticCommand.cs         # 语义化定义
├── Command/
│   ├── AccountCommand.cs          # 账户命令集合
│   ├── FileCommand.cs             # 文件命令集合
│   ├── TagCommand.cs              # 标签命令集合
│   └── TodoCommand.cs             # 待办命令集合
└── Domain.csproj
```

---

## 🔑 核心接口解读

### 1. CoreFoundation.cs - CQRS 基石

```csharp
// 所有请求的根接口
public interface IAppRequest { }

// 查询接口 - 只读操作
public interface IQuery<out TResponse> : IRequest<TResponse>, IAppRequest { }

// 命令接口 - 可自定义返回类型
public interface ICommand<out TResponse> : IRequest<TResponse>, IRequireTransaction, IAppRequest { }

// 默认命令 - 统一返回 Guid（推荐使用）
public interface ICommand : ICommand<Guid> { }
```

**设计优势：**
- `IQuery<T>` 清晰表达：这是一个只读查询
- `ICommand` 默认返回 Guid，表示创建/修改的实体 ID
- `ICommand<T>` 用于特殊场景（如登录返回 Token）

### 2. AspectMarkerInterfaces.cs - 标记接口

```csharp
// 标记接口 - 标识需要事务保护的请求
// TransactionBehavior 会在运行时检查这个标记
public interface IRequireTransaction { }
```

**使用场景：**
- 所有 `ICommand` 都继承 `IRequireTransaction`
- 在 TransactionBehavior 中检查：`if (request is IRequireTransaction)`

---

## 📋 命令定义示例

### Account 命令

```csharp
// CreateAccountCommand - 创建账户
public class CreateAccountCommand : ICommand
{
	public string Username { get; set; }
	public string Email { get; set; }
	public string PasswordHash { get; set; }
}

// LoginCommand - 登录（返回 Token）
public class LoginCommand : ICommand<string>
{
	public string Email { get; set; }
	public string Password { get; set; }
}

// UpdateAccountCommand - 更新账户
public class UpdateAccountCommand : ICommand
{
	public Guid AccountId { get; set; }
	public string Email { get; set; }
	public string DisplayName { get; set; }
}

// DeleteAccountCommand - 删除账户
public class DeleteAccountCommand : ICommand
{
	public Guid AccountId { get; set; }
}
```

### File 命令

```csharp
// CreateFileCommand - 上传文件
public class CreateFileCommand : ICommand
{
	public Guid UserId { get; set; }
	public string FileName { get; set; }
	public Stream FileStream { get; set; }
	public long FileSize { get; set; }
}

// DeleteFileCommand - 删除文件
public class DeleteFileCommand : ICommand
{
	public Guid FileId { get; set; }
	public Guid UserId { get; set; }
}

// UpdateFileTagCommand - 更新文件标签
public class UpdateFileTagCommand : ICommand
{
	public Guid FileId { get; set; }
	public List<Guid> TagIds { get; set; }
}
```

---

## 🔍 查询定义示例

### 查询命名约定

```csharp
// GetAccountQuery - 获取单个账户
public class GetAccountQuery : IQuery<AccountDto>
{
	public Guid AccountId { get; set; }
}

// QueryAccountsQuery - 查询多个账户
public class QueryAccountsQuery : IQuery<IEnumerable<AccountDto>>
{
	public int PageNumber { get; set; }
	public int PageSize { get; set; }
	public string SearchTerm { get; set; }
}

// GetFileMetadataQuery - 获取文件元数据
public class GetFileMetadataQuery : IQuery<FileDto>
{
	public Guid FileId { get; set; }
}

// PreviewFileQuery - 文件预览
public class PreviewFileQuery : IQuery<Stream>
{
	public Guid FileId { get; set; }
}

// DownloadFileQuery - 文件下载
public class DownloadFileQuery : IQuery<(Stream Stream, string FileName)>
{
	public Guid FileId { get; set; }
}
```

---

## 💡 设计最佳实践

### 1️⃣ 命令应该包含完整的操作信息

```csharp
// ✅ 好的做法 - 包含所有必要信息
public class CreateFileCommand : ICommand
{
	public Guid UserId { get; set; }
	public string FileName { get; set; }
	public byte[] FileContent { get; set; }
	public string MimeType { get; set; }
}

// ❌ 不好的做法 - 依赖上下文获取用户 ID
public class CreateFileCommand : ICommand
{
	public string FileName { get; set; }
	public byte[] FileContent { get; set; }
	// 缺少 UserId，需要从 HttpContext 获取
}
```

### 2️⃣ 查询不应该修改数据

```csharp
// ✅ 好的做法 - 只读查询
public class GetAccountQuery : IQuery<AccountDto>
{
	public Guid AccountId { get; set; }
}

// ❌ 不好的做法 - 查询中有修改操作
public class GetAndIncrementViewCountQuery : IQuery<AccountDto>
{
	public Guid AccountId { get; set; }
	// 这会导致查询意外修改数据
}
```

### 3️⃣ 使用统一的返回类型

```csharp
// ✅ 推荐：命令返回 Guid（修改的实体 ID）
public class CreateAccountCommand : ICommand { }  // 返回 Guid

// ✅ 允许：特殊场景返回自定义类型
public class LoginCommand : ICommand<LoginResponseDto> { }
public class QueryFilesCommand : IQuery<PagedResult<FileDto>> { }

// ❌ 避免：命令返回不相关的数据
public class CreateAccountCommand : ICommand<string> { }  // 为什么返回字符串？
```

### 4️⃣ 使用明确的命名约定

```
Command 命名：
  - Create{Entity}Command
  - Update{Entity}Command
  - Delete{Entity}Command
  - {Action}{Entity}Command（如 LoginCommand, PublishArticleCommand）

Query 命名：
  - Get{Entity}Query （获取单个）
  - Query{Entities}Query （查询多个）
  - {Action}{Entity}Query （如 PreviewFileQuery, SearchFilesQuery）
```

---

## 🔄 数据流

```
客户端请求
  ↓
Web Controller 创建 Command/Query 对象
  ↓
Controller.Send(command) 通过 MediatR 分发
  ↓
Application 层的对应 Handler 处理
  ↓
Handler 调用 Service 实现业务逻辑
  ↓
返回结果
```

---

## 📚 添加新命令/查询的步骤

### 步骤 1：在 Domain 层定义

```csharp
// Domain/Command/AccountCommand.cs
public class CreateAccountCommand : ICommand
{
	public string Email { get; set; }
	public string PasswordHash { get; set; }
}
```

### 步骤 2：在 Application 层创建 Handler

```csharp
// Application/Handlers/Account/CreateAccountHandler.cs
public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
{
	private readonly IAccountService _service;

	public CreateAccountCommandHandler(IAccountService service)
	{
		_service = service;
	}

	public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
	{
		return await _service.CreateAsync(request);
	}
}
```

### 步骤 3：在 Web 层的 Controller 中调用

```csharp
// Web/Controllers/AccountController.cs
[HttpPost]
public async Task<ActionResult<Guid>> Create([FromBody] CreateAccountCommand command)
{
	var accountId = await _mediator.Send(command);
	return CreatedAtAction(nameof(Get), new { id = accountId }, accountId);
}
```

---

## 🧪 单元测试示例

```csharp
[TestClass]
public class CreateAccountCommandTests
{
	[TestMethod]
	public void CreateAccountCommand_ShouldHaveRequiredFields()
	{
		// Arrange
		var command = new CreateAccountCommand
		{
			Email = "test@example.com",
			PasswordHash = "hashed_password"
		};

		// Assert
		Assert.IsNotNull(command.Email);
		Assert.IsNotNull(command.PasswordHash);
	}

	[TestMethod]
	public void CreateAccountCommand_ShouldImplementICommand()
	{
		// Arrange & Act
		var command = new CreateAccountCommand();

		// Assert
		Assert.IsInstanceOfType(command, typeof(ICommand));
	}
}
```

---

## ⚠️ 常见错误

### ❌ 错误 1：在 Domain 层放置业务逻辑

```csharp
// 不要这样做！
public class CreateAccountCommand : ICommand
{
	public void Validate() { /* 业务逻辑 */ }  // ❌ 错误位置
}

// 应该在 Application 层的 Validator 中处理
public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
	public CreateAccountCommandValidator()
	{
		// 验证规则
	}
}
```

### ❌ 错误 2：命令中包含复杂的对象

```csharp
// 不要这样做！
public class CreateFileCommand : ICommand
{
	public FileMetaEntity Entity { get; set; }  // ❌ 不应该包含 Entity
}

// 应该使用简单的数据类型
public class CreateFileCommand : ICommand
{
	public string FileName { get; set; }
	public long FileSize { get; set; }
	public byte[] FileContent { get; set; }
}
```

### ❌ 错误 3：修改 DTO 对象

```csharp
// 不要这样做！
public class GetAccountQuery : IQuery<AccountDto>
{
	public Guid AccountId { get; set; }

	public void UpdatePassword(string newPassword) { }  // ❌ 查询不应该有修改方法
}
```

---

## 📖 相关文档

- [README.md](README.md) - 项目总体说明
- [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) - 项目结构和改进建议
- [ARCHITECTURE_DECISIONS.md](ARCHITECTURE_DECISIONS.md) - 架构决策记录

---

## 🔗 链接

- **Application 层** - 处理这些命令和查询的 Handler
- **Service 层** - 实现具体的业务逻辑
- **MediatR 文档** - https://github.com/jbogard/MediatR

---

**版本：** 1.0  
**最后更新：** 2026年5月
