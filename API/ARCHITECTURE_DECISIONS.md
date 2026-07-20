# AqLife API - 架构决策记录（ADR）

## 格式说明

每个 ADR 包含以下部分：
- **标题** - 决策内容的简明描述
- **状态** - Proposed（提议）/ Accepted（已接受）/ Deprecated（已弃用）
- **背景** - 为什么需要这个决策
- **决策** - 我们的选择是什么
- **理由** - 为什么做出这个选择
- **后续** - 实施步骤和风险

---

## ADR-001：采用分层架构（Clean Architecture）

### 状态
✅ **已接受**（当前应用）

### 背景
系统需要：
- 高内聚、低耦合的代码组织
- 便于单元测试和模块化开发
- 清晰的关注点分离
- 易于维护和扩展

### 决策
采用 **6 层分层架构**：
1. **Web 层** - HTTP 入口（控制器、中间件）
2. **Application 层** - 业务流程协调（CQRS、管道）
3. **Domain 层** - 业务规则定义（命令、查询）
4. **Service 层** - 具体业务逻辑实现
5. **Data 层** - 数据持久化（ORM、迁移）
6. **Shared 层** - 跨层共享契约（异常、DTO）

### 理由
- ✅ 依赖方向清晰：高层依赖低层抽象
- ✅ 测试性强：各层独立测试
- ✅ 扩展性好：新功能在对应层添加
- ✅ 业界最佳实践：已被大型项目验证

### 后续
- 定期审查层级间的依赖关系
- 确保没有循环依赖
- 文档化每层的职责边界

---

## ADR-002：采用 CQRS（命令查询职责分离）模式

### 状态
✅ **已接受**（当前应用）

### 背景
系统需要：
- 统一的请求处理流程
- 清晰的读写分离
- 便于添加横切关注点（验证、事务、日志）
- 支持异步操作

### 决策
在 Application 层采用 **CQRS + MediatR**：
- **查询（Query）** - 只读操作，使用 `IQuery<TResponse>`
- **命令（Command）** - 修改操作，使用 `ICommand<TResponse>`
- **处理器（Handler）** - 通过 MediatR 自动分发

### 理由
- ✅ MediatR 是 .NET 生态最成熟的 CQRS 实现
- ✅ 自动分发和管道支持
- ✅ 易于添加通用行为（验证、事务、日志）
- ✅ 测试友好（可独立测试 Handler）

### 后续
- 统一命令/查询的命名约定
- 文档化常见的 Handler 模式
- 监控 MediatR 性能影响

---

## ADR-003：采用 MediatR Pipeline Behaviors 实现 AOP

### 状态
✅ **已接受**（当前应用）

### 背景
系统需要：
- 统一的验证流程（避免重复代码）
- 自动的事务管理（确保一致性）
- 可扩展的日志和监控
- 通用的异常处理

### 决策
实现两个 Pipeline Behavior：
1. **ValidationBehavior** - 在 Handler 前执行验证
2. **TransactionBehavior** - 为命令自动开启事务

```csharp
services.AddMediatR(cfg =>
{
	cfg.RegisterServicesFromAssemblies(implementationAssembly);
	cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
	cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
});
```

### 理由
- ✅ 避免在每个 Handler 中重复验证和事务代码
- ✅ 集中管理横切关注点
- ✅ 便于添加新的 Behavior（如日志、性能监控）
- ✅ 保持 Handler 代码清晰

### 后续
- 考虑添加 LoggingBehavior
- 考虑添加 PerformanceMonitoringBehavior
- 文档化 Behavior 的执行顺序

---

## ADR-004：数据库采用 MySQL + Entity Framework Core

### 状态
✅ **已接受**（当前应用）

### 背景
- 系统需要持久化存储
- 支持复杂的关系型数据
- 需要灵活的查询能力
- 团队已有 EF Core 经验

### 决策
- **数据库** - MySQL 8.0+
- **ORM** - Entity Framework Core 8
- **迁移** - Code-First 迁移方式

### 理由
- ✅ MySQL：开源、稳定、性能良好
- ✅ EF Core：官方支持、类型安全、LINQ 查询
- ✅ Code-First：数据库结构通过代码表达，便于版本控制

### 后续
- 定期审查数据库性能（加索引）
- 备份和恢复策略
- 版本升级计划

---

## ADR-005：JWT 用于身份认证

### 状态
✅ **已接受**（当前应用）

### 背景
- 系统需要支持多个客户端（Web、Mobile）
- 需要无状态的认证机制
- 支持跨域请求（CORS）

### 决策
采用 **JWT（JSON Web Token）**：
- Token 包含用户身份和权限信息
- 由 `AuthService` 生成
- 通过 HTTP Header `Authorization: Bearer <token>` 传递

### 理由
- ✅ 无状态：不需要服务器保存会话
- ✅ 可扩展：便于分布式部署
- ✅ 跨域友好：支持 CORS
- ✅ 标准化：JWT 是业界标准

### 后续
- 实现 Token 刷新机制
- Token 过期时间配置
- 黑名单管理（可选）

---

## ADR-006：策略模式用于灵活的搜索实现

### 状态
✅ **已接受**（当前应用在文件搜索中）

### 背景
- 文件搜索需要支持多种过滤条件
- 避免 if-else 地狱
- 易于添加新的搜索策略

### 决策
实现 **Strategy 模式**：
```csharp
public interface ISearchStrategy { }
public class AllFilesSearchStrategy : ISearchStrategy { }
public class FilteredFilesSearchStrategy : ISearchStrategy { }
```

### 理由
- ✅ 易于扩展：添加新策略只需要创建新类
- ✅ 易于测试：每个策略独立测试
- ✅ 易于维护：策略逻辑集中

### 后续
- 推广到其他搜索场景
- 考虑使用工厂模式创建策略
- 文档化各个策略的使用场景

---

## ADR-007：异常体系设计

### 状态
🟡 **部分实现**（需要完善）

### 背景
- 系统需要清晰的错误分类
- API 响应应该有统一的错误格式
- 便于上层捕获和处理特定异常

### 决策（当前）
定义了 5 种异常：
- `BusinessException` - 业务逻辑违反
- `DatabaseException` - 数据库错误
- `FileException` - 文件操作错误
- `OptionException` - 配置错误
- `RequestException` - 请求错误

### 决策（建议改进）
扩展到 HTTP 状态码对应的异常：
- `ValidationException` - 400 Bad Request
- `UnauthorizedException` - 401 Unauthorized
- `ForbiddenException` - 403 Forbidden
- `EntityNotFoundException` - 404 Not Found
- `BusinessException` - 422 Unprocessable Entity
- `DatabaseException` - 500 Internal Server Error

### 理由
- ✅ 每个异常对应一个 HTTP 状态码
- ✅ 便于异常中间件统一处理
- ✅ API 客户端可以根据状态码确定错误类型

### 后续
- 统一 API 错误响应格式
- 文档化错误代码表
- 国际化错误消息

---

## ADR-008：验证器集中管理

### 状态
✅ **已接受**（当前应用）

### 背景
- 验证规则较多且复杂
- 需要重用验证规则
- 需要统一的验证流程

### 决策
- **位置** - Application 层的 Validators 目录
- **分类**：
  - `AbstractValidator` - 验证器基类
  - `ExistenceValidator` - 实体存在性验证
  - `BusinessValidator/*` - 业务规则验证
- **集成** - 通过 ValidationBehavior 自动执行

### 理由
- ✅ 验证逻辑集中管理，易于维护
- ✅ 可以重用验证规则
- ✅ 自动集成到 CQRS 流程

### 后续
- 添加自定义验证特性
- 支持跨字段验证
- 国际化验证消息

---

## ADR-009：DTO 映射策略

### 状态
✅ **已接受**（当前应用）

### 背景
- 需要将 Entity 转换为 DTO（隐藏内部实现）
- 避免 Entity 直接暴露给客户端
- 支持多种映射场景（不同的 API 版本）

### 决策
在 Service 层实现 **Mapper**：
```csharp
public class AccountMapper : IMapper
{
	public AccountDto Map(AccountEntity entity) { }
}
```

### 理由
- ✅ 明确的职责边界
- ✅ 易于版本化（不同 API 版本用不同 Mapper）
- ✅ 便于单元测试

### 后续
- 考虑使用 AutoMapper 减少样板代码
- 文档化映射规则
- 性能优化（特别是大量数据转换）

---

## ADR-010：文件上传/下载策略

### 状态
✅ **已接受**（当前应用）

### 背景
- 系统需要处理文件上传和下载
- 需要安全的文件访问控制
- 需要性能优化（大文件处理）

### 决策
- **上传** - 存储在服务器文件系统（Web/Cache）
- **下载** - 通过 FileController 提供
- **访问控制** - FilePolicyFilter 中间件检查权限
- **元数据** - 存储在 FileMetaEntity 表中

### 理由
- ✅ 简单直接
- ✅ 支持访问控制
- ✅ 便于性能优化（CDN 集成）

### 后续
- 考虑迁移到对象存储（OSS）
- 实现文件预览功能
- 病毒扫描集成
- 大文件分片上传

---

## ADR-011：日志管理

### 状态
🟡 **部分实现**（推荐改进）

### 背景
- 系统需要记录重要事件和错误
- 便于问题诊断和审计

### 当前实现
- 日志文件存储在 Web/logs 目录
- 缺少结构化日志配置

### 建议改进
- 集成 **Serilog** - 结构化日志框架
- 支持多个 Sink：
  - **File** - 本地文件
  - **Console** - 控制台输出
  - **Database** - 数据库存储
  - **CloudLogging** - 云日志服务

### 后续
- 日志级别配置
- 敏感信息脱敏
- 日志查询和分析

---

## ADR-012：配置管理

### 状态
✅ **已接受**（当前应用）

### 背景
- 不同环境有不同配置（开发、测试、生产）
- 敏感信息（密码、密钥）不应上传到版本控制

### 决策
- **配置文件** - appsettings.{Environment}.json
- **敏感信息** - 通过环境变量或密钥库
- **示例文件** - appsettings.*.example.json

### 理由
- ✅ 标准做法（.NET 原生支持）
- ✅ 灵活的环境配置
- ✅ 安全性（敏感信息不上传版本控制）

### 后续
- 集成密钥管理服务（Azure Key Vault / AWS Secrets Manager）
- 运行时配置更新支持
- 配置验证和加密

---

## ADR-013：RESTful API 设计约定

### 状态
✅ **已接受**（当前应用）

### 背景
- API 需要遵循 RESTful 原则
- 便于客户端集成和理解

### 决策
- **资源** - 以名词表示（/accounts, /files, /tags）
- **方法** - 使用 HTTP 方法表示操作（GET, POST, PUT, DELETE）
- **状态码** - 遵循 HTTP 状态码约定
- **分页** - 通过查询参数实现

### 理由
- ✅ 业界标准
- ✅ 直观易用
- ✅ 便于文档化

### 后续
- API 版本控制策略（/api/v1 vs /api/v2）
- API 文档自动生成（Swagger/OpenAPI）
- 性能优化（缓存、速率限制）

---

## 决策矩阵

| 决策 | 优先级 | 风险 | 复杂度 | 状态 |
|------|--------|------|--------|------|
| ADR-001 分层架构 | 🔴 高 | 低 | 中 | ✅ 已实施 |
| ADR-002 CQRS 模式 | 🔴 高 | 低 | 中 | ✅ 已实施 |
| ADR-003 Pipeline Behaviors | 🔴 高 | 低 | 低 | ✅ 已实施 |
| ADR-004 MySQL + EF Core | 🔴 高 | 低 | 中 | ✅ 已实施 |
| ADR-005 JWT 认证 | 🔴 高 | 低 | 低 | ✅ 已实施 |
| ADR-006 策略模式 | 🟡 中 | 低 | 低 | ✅ 已实施 |
| ADR-007 异常体系 | 🟡 中 | 低 | 中 | 🟡 部分 |
| ADR-008 验证器集中管理 | 🟡 中 | 低 | 中 | ✅ 已实施 |
| ADR-009 DTO 映射 | 🟡 中 | 低 | 低 | ✅ 已实施 |
| ADR-010 文件管理 | 🟡 中 | 中 | 中 | ✅ 已实施 |
| ADR-011 日志管理 | 🟢 低 | 低 | 中 | 🟡 部分 |
| ADR-012 配置管理 | 🔴 高 | 低 | 低 | ✅ 已实施 |
| ADR-013 RESTful 设计 | 🔴 高 | 低 | 低 | ✅ 已实施 |

---

## 后续回顾计划

- **每月** - 回顾新发现的架构问题
- **每季度** - 审视决策是否仍然适用
- **每年** - 全面的架构审计和改进规划

---

**文档版本：** 1.0  
**最后更新：** 2026年5月  
**下一次审查：** 2026年8月
