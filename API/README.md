# AqLife API - 项目结构与设计文档

## 📋 项目概述

AqLife API 是一个现代化的 .NET 8 后端系统，采用 **清洁架构（Clean Architecture）** 和 **CQRS（命令查询职责分离）** 模式设计。项目分为多个职责明确的层级，每层各司其职，形成高内聚、低耦合的体系结构。

**核心特性：**
- ✅ 基于 MediatR 的 CQRS 架构
- ✅ 分层设计（Web → Application → Service → Data）
- ✅ 事务和验证 Pipeline 行为
- ✅ JWT 身份认证与授权
- ✅ 全局异常处理
- ✅ Entity Framework Core + MySQL ORM
- ✅ 文件上传/下载管理
- ✅ 灵活的搜索策略模式

---

## 📐 项目整体架构

```
┌─────────────────────────────────────────────────────────────┐
│                     Web Layer (Web.csproj)                  │
│              HTTP 入口、路由、中间件、控制器                  │
└────────────────────────┬────────────────────────────────────┘
						 │
┌────────────────────────▼────────────────────────────────────┐
│              Application Layer (Application.csproj)          │
│          CQRS 请求处理、验证行为、事务管理、Handler            │
└────────────────────────┬────────────────────────────────────┘
						 │
   ┌─────────────────────┼─────────────────────┐
   │                     │                     │
┌──▼──────────┐ ┌───────▼────────┐ ┌────────▼──────┐
│ Service.cs  │ │ Domain.cs      │ │  Data.cs      │
│ (Entity     │ │ (Command &     │ │ (Repository & │
│  Services)  │ │  Query Logic)  │ │   Entities)   │
└──────────────┘ └────────────────┘ └───────────────┘
						 │
					┌────▼─────┐
					│ Shared.cs │
					│ (Common   │
					│ Contracts)│
					└──────────┘
```

---

## 🏗️ 各层级职责详解

### 1️⃣ **Web 层** (`API/Web/`)

**职责：** HTTP 请求的入口点，负责接收、路由、返回响应。

#### 目录结构：
```
Web/
├── Controllers/              # HTTP 控制器（6 个业务域）
│   ├── AccountController.cs      # 账户管理（登录、注册、更新）
│   ├── BlogController.cs         # 博客/内容管理
│   ├── CorpusController.cs       # 语料库管理
│   ├── FileController.cs         # 文件上传/下载/预览
│   ├── TagController.cs          # 标签管理
│   └── TodoController.cs         # 待办事项管理
├── Middlewares/              # HTTP 中间件
│   ├── ExceptionHandler.cs       # 全局异常捕获与格式化
│   └── FilePolicyFilter.cs       # 文件访问策略控制
├── Extensions/               # 依赖注入与配置扩展
│   ├── InfrastructureSetup.cs    # 基础设施配置
│   ├── JwtSetup.cs               # JWT 认证配置
│   ├── ExceptionHandlerSetup.cs  # 异常处理配置
│   ├── FilePolicy.cs             # 文件策略配置
│   └── RoutingSetup.cs           # 路由适配器配置
├── Configurations/           # 配置文件
│   ├── appsettings.json           # 默认配置
│   ├── appsettings.Development.example.json
│   ├── appsettings.Production.example.json
│   └── FilePolicy.*.json          # 分环境文件策略配置
├── Cache/                    # 缓存目录（临时文件存储）
├── logs/                     # 日志输出目录
├── Program.cs                # 应用启动配置
└── Web.http                  # REST 客户端测试文件
```

#### 关键职责：
- 🔌 解析 HTTP 请求参数
- 📤 调用 Application 层的 MediatR Handler
- 🛡️ 依赖 JWT 中间件进行身份认证
- 📋 应用文件访问策略
- 🚨 通过异常处理中间件统一格式化响应

#### 设计要点：
- Controllers 应保持**瘦弱**（只做参数验证和响应转换）
- 业务逻辑全部下沉到 Application 层
- 所有跨切面关注点（认证、异常、日志）通过中间件统一处理

---

### 2️⃣ **Application 层** (`API/Application/`)

**职责：** 应用业务流程协调，基于 CQRS 模式处理所有业务请求。

#### 目录结构：
```
Application/
├── Handlers/                 # CQRS 命令/查询处理器（按业务域分组）
│   ├── Account/              # 账户处理（Create/Delete/Get/Login/Update）
│   ├── File/                 # 文件处理（Create/Delete/Download/Preview/UpdateTag）
│   ├── Tag/                  # 标签处理（Create/Delete/Query/Update）
│   └── (未来可扩展更多域)
├── Behaviors/                # MediatR Pipeline 行为（AOP）
│   ├── ValidationBehavior.cs # 验证管道（自动执行验证器）
│   └── TransactionBehavior.cs# 事务管道（为命令自动开启事务）
├── Validators/               # 验证器
│   ├── AbstractValidator.cs           # 验证器基类
│   ├── ExistenceValidator.cs          # 存在性验证（如检查用户是否存在）
│   └── BusinessValidator/
│       ├── AccountValidator.cs        # 账户业务规则验证
│       ├── FileValidator.cs           # 文件业务规则验证
│       ├── TagValidator.cs            # 标签业务规则验证
│       └── TodoValidator.cs           # 待办事项业务规则验证
└── Application.cs            # DI 配置入口
```

#### 工作流程：
```
HTTP Request
	↓
Controller (参数转换)
	↓
MediatR.Send(IRequest) 
	↓
ValidationBehavior (验证请求)
	↓
(验证失败 → 异常 → 中间件处理)
	↓
TransactionBehavior (开启事务)
	↓
对应的 Handler 执行业务逻辑
	↓
return Guid/Result
```

#### 关键职责：
- ✅ 协调各层完成业务流程
- ✅ 统一验证（通过 ValidationBehavior）
- ✅ 统一事务管理（通过 TransactionBehavior）
- ✅ 返回操作结果（Guid 或自定义类型）

#### 设计要点：
- **Handler 应该是业务流程编排器**，实际逻辑委托给 Service 层
- **Validator 集中管理验证规则**，避免验证逻辑分散
- **Pipeline Behaviors 实现通用的横切关注点**

---

### 3️⃣ **Domain 层** (`API/Domain/`)

**职责：** 定义 CQRS 的核心接口和命令/查询语义。

#### 目录结构：
```
Domain/
├── CommandInterface/
│   ├── CoreFoundation.cs       # 核心 CQRS 接口基石
│   │   ├── IAppRequest         # 所有请求的顶级接口
│   │   ├── IQuery<TResponse>   # 查询接口（只读）
│   │   ├── ICommand<TResponse> # 命令接口（可自定义返回类型）
│   │   └── ICommand            # 默认命令（统一返回 Guid）
│   ├── AspectMarkerInterfaces.cs # 标记接口（用于反射识别）
│   │   └── IRequireTransaction # 标记需要事务保护的请求
│   └── SemanticCommand.cs      # 语义化命令定义（业务命令）
└── Command/                    # 具体的命令实现（按业务域）
	├── AccountCommand.cs       # 账户相关命令
	├── FileCommand.cs          # 文件相关命令
	├── TagCommand.cs           # 标签相关命令
	└── TodoCommand.cs          # 待办事项相关命令
```

#### CQRS 设计规范：
- **IQuery<T>：** 只读查询，不修改数据
- **ICommand：** 修改数据的命令，统一返回操作的实体 ID（Guid）
- **ICommand<T>：** 特殊场景下允许自定义返回类型

#### 关键职责：
- 📝 定义业务领域的命令与查询接口
- 🏷️ 提供语义化的命令/查询实现
- 🔄 标记跨切面关注点（如事务要求）

#### 设计要点：
- **接口设计应该表达业务语义**，如 `CreateAccountCommand`, `QueryTodoByUserIdQuery`
- **充分利用泛型约束**，在编译期发现问题
- **标记接口应该被用来驱动行为**（如 TransactionBehavior 检查 IRequireTransaction）

---

### 4️⃣ **Service 层** (`API/Service/`)

**职责：** 具体的业务逻辑实现、数据映射、搜索策略等。

#### 目录结构：
```
Service/
├── EntityService/            # 实体业务逻辑服务
│   ├── AccountService.cs       # 账户操作逻辑
│   ├── FileService.cs          # 文件操作逻辑
│   ├── CorpusService.cs        # 语料库操作逻辑
│   ├── TagServices.cs          # 标签操作逻辑
│   ├── TodoServices.cs         # 待办事项操作逻辑
│   └── SearchSpecification.cs  # 搜索规范定义
├── Features/
│   └── AuthService.cs          # JWT 提供者实现（认证服务）
├── Interfaces/                # 服务接口
│   ├── IJwtProvider.cs         # JWT 认证接口
│   ├── IMapper.cs              # 映射接口
│   └── IStrategy/
│       └── ISearchStrategy.cs  # 搜索策略模式接口
├── Mapper/                    # DTO 映射器（Entity ↔ DTO）
│   ├── AccoutMapper.cs         # 账户映射
│   ├── FileMapper.cs           # 文件映射
│   ├── TagMapper.cs            # 标签映射
│   └── TodoMapper.cs           # 待办事项映射
├── Search/                    # 搜索实现（策略模式）
│   └── File/
│       ├── FileSearch.cs       # 文件搜索协调器
│       └── Strategy/
│           ├── AllFilesSearchStrategy.cs       # 查询全部文件
│           └── FilteredFilesSearchStrategy.cs  # 按条件过滤查询
└── Service.cs                 # DI 配置入口
```

#### 设计模式：

**1. 策略模式（Search）**
```csharp
// 定义策略接口
public interface ISearchStrategy { }

// 具体策略实现
public class AllFilesSearchStrategy : ISearchStrategy { }
public class FilteredFilesSearchStrategy : ISearchStrategy { }

// 协调器使用策略
public class FileSearch
{
	public void Execute(ISearchStrategy strategy) { }
}
```

**2. Mapper 模式（DTO 转换）**
```csharp
// Service 层负责将实体映射为 DTO
public class FileMapper : IMapper
{
	public FileDto Map(FileMetaEntity entity) { }
}
```

#### 关键职责：
- 🧮 实现具体的业务逻辑计算
- 🔄 数据映射（Entity → DTO）
- 🔍 支持灵活的搜索和过滤
- 🔐 JWT 令牌生成与验证
- 📊 聚合来自 Data 层的数据

#### 设计要点：
- **Service 应该是无状态的**（便于并发和扩展）
- **策略模式提供灵活的搜索支持**，避免 if-else 地狱
- **Mapper 集中管理数据转换规则**，提高可维护性

---

### 5️⃣ **Data 层** (`API/Data/`)

**职责：** 数据库访问、ORM 映射、数据持久化。

#### 目录结构：
```
Data/
├── Entities/                 # EF Core 数据模型
│   ├── AccountEntity.cs       # 账户表映射
│   ├── TodoEntity.cs          # 待办事项表映射
│   ├── FileMetaEntity.cs      # 文件元数据表映射
│   ├── FileTagEntity.cs       # 文件-标签关联表映射
│   ├── TagEntity.cs           # 标签表映射
│   └── CorpusEntity.cs        # 语料库表映射
├── Repository/               # 数据访问
│   └── AppStorage.cs          # DbContext（EF Core 数据库上下文）
├── Migrations/                # 数据库迁移历史（22+ 个迁移文件）
│   ├── 20260506070312_InitialCreate.cs
│   ├── 20260508033306_OptimizeDataStructures.cs
│   ├── 20260626092512_OptimizeFile.cs
│   └── ... (数据库演进历史记录)
└── DataLayerSetup.cs          # DI 配置入口
```

#### 数据库架构：
- **DbContext：** `AppStorage` - 所有表的单一入口
- **ORM 框架：** Entity Framework Core 8
- **数据库：** MySQL（版本通过配置文件指定）
- **迁移管理：** Code-First 迁移方式

#### 关键职责：
- 📦 定义所有数据表的实体模型
- 💾 管理 DbContext 生命周期
- 🔄 提供 ORM 查询接口（LINQ）
- 📝 版本化管理数据库结构变更

#### 设计要点：
- **Entity 应仅包含必要的属性和关系配置**
- **AppStorage DbContext 应该只负责数据访问**，不实现业务逻辑
- **迁移文件应该保存历史**，便于数据库版本回溯

---

### 6️⃣ **Shared 层** (`API/Shared/`)

**职责：** 跨层共享的契约、工具、异常和配置。

#### 目录结构：
```
Shared/
├── Contracts/                # 接口契约（跨层协议）
│   ├── IEntity.cs            # 实体接口（所有实体遵循）
│   ├── IUserEntity.cs        # 用户相关实体接口
│   ├── ISimpleAccountInfo.cs # 简化的账户信息接口
│   └── IValidator.cs         # 验证器接口
├── Exceptions/                # 自定义异常类型
│   ├── BusinessException.cs   # 业务异常
│   ├── DataBaseException.cs   # 数据库异常
│   ├── FileException.cs       # 文件操作异常
│   ├── OptionException.cs     # 配置选项异常
│   └── RequestException.cs    # 请求异常
├── IView/                     # DTO 定义（API 响应数据）
│   ├── IEntityDto.cs          # DTO 基接口
│   ├── AccountDto.cs          # 账户 DTO
│   ├── FileDto.cs             # 文件 DTO
│   ├── TagDto.cs              # 标签 DTO
│   ├── TodoDto.cs             # 待办 DTO
│   └── SubscriptionDto.cs     # 订阅 DTO
├── Options/                   # 配置选项类
│   ├── DbOption.cs            # 数据库配置
│   ├── JwtOption.cs           # JWT 配置
│   ├── FileOption.cs          # 文件配置
│   └── APIStatus.cs           # API 状态码定义
├── Tools/                     # 工具函数
│   ├── GetUserID.cs           # 获取当前用户 ID
│   ├── GetFiles.cs            # 获取文件列表工具
│   └── UploadContext.cs       # 文件上传上下文
└── Utils/                     # 通用工具
	├── FastHash.cs            # 快速哈希实现
	└── IdentityGenerator.cs   # ID 生成器
```

#### 关键职责：
- 📋 定义各层需要遵循的接口契约
- ⚡ 提供通用工具和辅助函数
- 🚨 自定义异常类型体系
- ⚙️ 配置选项的类型定义

#### 设计要点：
- **Contracts 应该是最小化的抽象**，避免过度设计
- **异常应该有明确的类型和消息**，便于上层处理
- **DTO 应该对应 API 的实际响应格式**
- **Options 应该与 appsettings.json 结构对应**

---

## 🔄 数据流与通信模式

### 完整请求周期：

```
1. 客户端发送 HTTP 请求
   ↓
2. Web 层控制器接收请求
   ├─ 参数验证（ModelState）
   ├─ JWT 中间件检查认证
   └─ 文件策略中间件检查访问权限
   ↓
3. 控制器创建 Command/Query 对象
   ↓
4. 调用 MediatR.Send() 分发请求
   ↓
5. ValidationBehavior
   ├─ 获取对应的 Validator
   ├─ 执行验证规则
   └─ 验证失败抛出异常
   ↓
6. TransactionBehavior（仅命令）
   ├─ 检查 IRequireTransaction 标记
   ├─ 开启数据库事务
   └─ 准备异常回滚机制
   ↓
7. 对应的 Handler 执行
   ├─ 调用 Service 实现业务逻辑
   ├─ Service 调用 Data 层访问数据库
   ├─ Service 进行数据映射（Entity → DTO）
   └─ 返回结果
   ↓
8. TransactionBehavior 提交事务
   ↓
9. 控制器返回 HTTP 响应
   ↓
10. 客户端收到响应

异常处理：
  ✗ 验证失败 → ValidationException → 异常中间件 → 400 Bad Request
  ✗ 业务规则违反 → BusinessException → 异常中间件 → 422 Unprocessable Entity
  ✗ 数据库错误 → DataBaseException → 异常中间件 → 500 Internal Server Error
```

---

## 🏢 业务域分析

### 1. **Account 域（账户管理）**
- **命令：** CreateAccountCommand, LoginCommand, UpdateAccountCommand, DeleteAccountCommand
- **查询：** GetAccountQuery
- **服务：** AccountService（关键方法：Create, Update, Delete, Get）
- **验证：** AccountValidator

### 2. **File 域（文件管理）**
- **命令：** CreateFileCommand, DeleteFileCommand, UpdateFileTagCommand
- **查询：** GetFileMetadataQuery, PreviewFileQuery, DownloadFileQuery
- **服务：** FileService（关键方法：Upload, Delete, GetMetadata, Preview）
- **验证：** FileValidator
- **策略：** AllFilesSearchStrategy, FilteredFilesSearchStrategy

### 3. **Tag 域（标签管理）**
- **命令：** CreateTagCommand, DeleteTagCommand, UpdateTagCommand
- **查询：** TagQueryQuery（可能是笔误，应为 QueryTagsQuery）
- **服务：** TagServices
- **验证：** TagValidator

### 4. **Todo 域（待办事项）**
- **命令：** 创建、更新、删除、完成
- **查询：** 按用户查询、按状态查询
- **服务：** TodoServices
- **验证：** TodoValidator

### 5. **Blog/Corpus 域（内容管理）**
- **功能：** 博客文章和语料库管理
- **服务：** CorpusService

---

## 🔧 依赖关系图

```
Web (HTTP 入口)
  ↓
  ├→ Application (CQRS + Pipeline)
  │   ├→ Domain (Command/Query 定义)
  │   ├→ Service (业务逻辑)
  │   └→ Shared (异常、验证器)
  │
  ├→ Service (业务实现)
  │   ├→ Data (数据访问)
  │   ├→ Shared (DTO、工具)
  │   └→ Domain (Command/Query)
  │
  └→ Data (数据库)
	  └→ Shared (Entity 接口)
```

### 依赖方向原则：
- ✅ 高层依赖抽象（接口）
- ✅ 底层只实现具体逻辑
- ❌ 禁止循环依赖
- ❌ 禁止底层依赖高层

---

## 📊 当前项目状态诊断

### ✅ 优势：
1. **架构清晰** - 分层明确，关注点分离
2. **CQRS 规范** - 命令查询分离，便于扩展
3. **管道化处理** - 验证、事务统一管理
4. **异常体系完整** - 自定义异常便于诊断
5. **文件管理完善** - 有专门的文件处理层和访问策略
6. **搜索策略灵活** - 使用策略模式支持多种搜索方式

### ⚠️ 可改进的地方：
1. **命名不一致** 
   - 文件：`DataLayerSetup .cs` 有多余空格
   - 类名：`TagQueryHandler`（应为 `QueryTagsHandler`）
   - 拼写：`DowanloadFileHandler`（应为 `DownloadFileHandler`）

2. **缺少接口抽象**
   - EntityService 类可能缺少接口定义
   - 不利于单元测试和依赖注入

3. **文档不完整**
   - 缺少 API 文档（推荐使用 Swagger 增强）
   - 缺少快速启动指南

4. **日志管理**
   - 日志文件存在于 Web 项目的 logs 目录
   - 缺少结构化日志配置（推荐 Serilog）

5. **缓存策略**
   - Cache 目录用于临时文件存储
   - 缺少分布式缓存（Redis）支持

### 🎯 建议改进方向：
1. **接口隔离** - 为 EntityService 定义接口
2. **日志统一** - 使用 Serilog 实现结构化日志
3. **API 文档** - 增强 Swagger 注解
4. **单元测试** - 添加 xUnit/NUnit 测试项目
5. **性能优化** - 引入分布式缓存和异步处理
6. **监控告警** - 集成应用性能监控（APM）

---

## 🚀 快速启动指南

### 前置条件：
- .NET 8 SDK
- MySQL 8.0+
- Visual Studio 2022+

### 启动步骤：
1. **克隆仓库**
   ```bash
   git clone https://github.com/AQLIFE/AqLife.git
   cd AqLife
   ```

2. **配置数据库**
   ```bash
   cp API/Web/Configurations/appsettings.Development.example.json \
	  API/Web/Configurations/appsettings.Development.json
   # 编辑 appsettings.Development.json 配置 MySQL 连接字符串
   ```

3. **应用数据库迁移**
   ```bash
   cd API
   dotnet ef database update --project Data --startup-project Web
   ```

4. **启动应用**
   ```bash
   cd API
   dotnet run --project Web
   ```
   访问：http://localhost:5000/swagger

---

## 📚 关键类和接口说明

| 组件 | 位置 | 职责 |
|------|------|------|
| `IAppRequest` | Domain/CoreFoundation | 所有请求的根接口 |
| `IQuery<T>` | Domain/CoreFoundation | 查询请求接口 |
| `ICommand` | Domain/CoreFoundation | 命令请求接口 |
| `ValidationBehavior<,>` | Application/Behaviors | 自动验证管道 |
| `TransactionBehavior<,>` | Application/Behaviors | 自动事务管道 |
| `AppStorage` | Data/Repository | 数据库上下文 |
| `IMapper` | Service/Interfaces | DTO 映射器接口 |
| `ISearchStrategy` | Service/Interfaces | 搜索策略接口 |
| `IJwtProvider` | Service/Interfaces | JWT 令牌提供者 |
| `AuthService` | Service/Features | JWT 认证实现 |

---

## 🔐 安全特性

- ✅ JWT 令牌认证
- ✅ 基于策略的授权
- ✅ 文件访问控制（FilePolicyFilter）
- ✅ 全局异常处理（避免信息泄露）
- ✅ 输入验证（在 Application 层统一执行）
- ✅ SQL 注入防护（使用 EF Core 参数化查询）

---

## 📞 常见问题

**Q: 如何添加新的业务域？**
A: 
1. 在 Domain/Command 中定义命令类
2. 在 Application/Handlers 中创建 Handler
3. 在 Service 中实现业务逻辑
4. 在 Web/Controllers 中添加控制器

**Q: 如何自定义验证规则？**
A: 
1. 创建类继承 `AbstractValidator<T>`
2. 在验证器中定义规则
3. 自动被 ValidationBehavior 拾取并执行

**Q: 如何添加新的数据库表？**
A:
1. 在 Data/Entities 中定义 Entity 类
2. 在 AppStorage DbContext 中添加 DbSet 属性
3. 运行迁移：`dotnet ef migrations add MigrationName`
4. 应用迁移：`dotnet ef database update`

**Q: 搜索策略如何工作？**
A: 
- 定义策略类实现 `ISearchStrategy`
- FileSearch 类根据条件选择对应的策略
- 执行查询逻辑并返回结果

---

## 📝 版本历史

- **v1.0** - 初始版本，包含基础 CQRS 架构
- 数据库经历 15+ 次迁移优化，持续演进中

---

## 📄 许可证

[待填充 - 根据实际项目许可证填写]

---

## 🤝 贡献指南

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/amazing-feature`)
3. 提交更改 (`git commit -m 'Add amazing feature'`)
4. 推送到分支 (`git push origin feature/amazing-feature`)
5. 开启 Pull Request

---

**最后更新：** 2026年（基于当前项目状态生成）

**项目维护者：** AqLife 开发团队
