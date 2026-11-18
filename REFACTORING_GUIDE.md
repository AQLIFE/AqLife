# 后端模块化单体重构设计指南

> 本文是 AqLife 后端重构的设计基线与实施指南。当前目标是**模块化单体（Modular Monolith）**，不是微服务。本文只规定目标架构、依赖边界、迁移步骤和验收标准；在完成基线复核前，不应将其视为已经实施的代码变更。

## 1. 重构目标与范围

### 1.1 当前痛点

当前后端 Application 层以较大的业务目录组织代码。修改一个业务用例或新增类型时，往往需要在多个横向目录中查找 Handler、Validator、Mapper、Search 等文件，增加了人工检索成本。与此同时，业务模块之间存在直接实现依赖，公共持久化接口暴露了多个业务的数据集，导致仅靠目录划分无法形成可靠的边界。

### 1.2 目标

- 将每个主要业务拆分为独立的 Application `.csproj`，以编译时项目引用表达依赖边界。
- 每个业务内部采用 Feature Folder / Vertical Slice（按功能切片）组织，使一个用例相关的 Command、Handler、Validator 等文件集中在一起。
- 通过 Abstractions 中的接口或事件进行跨模块协作，避免业务模块直接引用其他业务模块的实现。
- 保留单一 API 入口、单一部署单元、单一数据库和集中 EF Core DbContext / migrations。
- 为未来可能的服务拆分保留清晰边界，但不以微服务化为当前目标。
- 逐步建立架构测试，防止项目引用和依赖方向退化。

### 1.3 非目标

本次重构不包括：
- 将 Account、Todo、File 等业务分别变成独立 Web API 或独立进程。
- 为每个业务模块建立独立数据库或独立迁移链。
- 引入服务注册中心、API Gateway、分布式消息总线或远程 HTTP/gRPC 模块调用。
- 为了项目拆分而改变现有业务行为、API 契约或数据库结构。
- 将目录移动、项目拆分与业务缺陷修复混在同一个提交中。

## 2. 模块化单体，不是微服务

独立 `.csproj` 代表独立的编译单元和依赖边界，不代表独立部署单元。各 Application 模块最终由同一个 ASP.NET Core Web API 加载，运行在同一个进程中。

| 维度 | 本方案：模块化单体 | 微服务 |
|---|---|---|
| 部署 | 一个 API 统一部署 | 多个服务独立部署 |
| 运行 | 通常一个 API 进程 | 多个服务进程 |
| 数据 | 共用数据库，逐步约束模块数据访问 | 通常各服务拥有自己的数据存储 |
| 模块通信 | 进程内接口、方法调用、MediatR | HTTP、gRPC、消息队列等 |
| 项目边界 | 多个 Application 项目 | 多个独立服务 |
| 运维复杂度 | 相对较低 | 相对较高 |

当前应坚持：一个 API、一个数据库、一套集中迁移、一个部署单元。只有当某个模块确实需要独立扩容、部署、运维或数据自治时，才重新评估是否抽取为微服务。

## 3. 目标解决方案结构

建议在 `services/API` 下逐步演进为：

```text
services/API/
  Domain/
  Shared/
  Application/
    Abstractions/
      AqLife.Application.Abstractions.csproj
    Core/
      AqLife.Application.Core.csproj
    Modules/
      Account/
        AqLife.Application.Account.csproj
      Todo/
        AqLife.Application.Todo.csproj
      File/
        AqLife.Application.File.csproj
      Favorite/
        AqLife.Application.Favorite.csproj
      Category/
        AqLife.Application.Category.csproj
      Tag/
        AqLife.Application.Tag.csproj
      Corpus/
        AqLife.Application.Corpus.csproj
  Infrastructure/
  Extensions/                 # 可逐步明确为 Bootstrap / Composition Root
  PostgreSqlMigrations/
  Web/
  CoreTest/                   # 可逐步按模块拆分测试项目
```

### 3.1 项目职责

| 项目 | 职责 |
|---|---|
| Application.Abstractions | 跨模块契约、持久化端口、应用层标记接口、必要的共享接口 |
| Application.Core | 应用层通用能力，例如共享 Pipeline、基础 Search 能力及公共用例基础设施 |
| Application.Account | 账户、认证、资料、订阅相关用例 |
| Application.Todo | 任务、开发计划、任务依赖、路线图/布局相关用例 |
| Application.File | 文件上传、存储、发布、预览、删除、标签关联等用例 |
| Application.Favorite | 收藏条目、媒体、标签和查询 |
| Application.Category | 分类管理和查询 |
| Application.Tag | 标签管理、查询及关联 |
| Application.Corpus | Corpus 管理、查询和随机内容等用例 |
| Infrastructure | EF Core 实现、统一 AppStorage、文件存储等基础设施实现 |
| Extensions / Bootstrap | 组合根：注册各模块、Infrastructure、MediatR 与 Pipeline |
| Web | ASP.NET Core 启动项目、Controllers、HTTP 配置 |
| PostgreSqlMigrations | 集中管理 PostgreSQL EF Core migrations |

模块数量和边界应以依赖审查结果为准；若某些模块在当前代码中尚无足够独立性，可先保持原状，不为追求数量而拆分。

## 4. 项目引用与依赖规则

目标引用关系：

- `Application.Abstractions -> Domain, Shared`
- `Application.Core -> Application.Abstractions, Domain, Shared`
- 每个业务模块 -> `Application.Core, Application.Abstractions, Domain, Shared`
- 业务模块之间**不得直接 ProjectReference 到其他业务模块**。
- `Infrastructure -> Application.Abstractions, Domain, Shared`，不引用业务 Application 实现项目。
- `Extensions / Bootstrap -> 所有业务模块 + Infrastructure`。
- `Web -> Bootstrap + 需要暴露请求/查询类型的模块 + Shared + PostgreSqlMigrations`。
- `PostgreSqlMigrations -> Infrastructure`（沿用当前迁移宿主关系）。

依赖方向应保持单向：Web 负责入口，Bootstrap 负责组装，业务模块依赖抽象和核心能力，Infrastructure 实现端口。不要让 Infrastructure 反向依赖业务用例实现。

## 5. 模块内部组织：Feature Folder / Vertical Slice

不再把一个业务的所有 Handler、Validator、Mapper、Search 横向分散在同级目录。优先按用户可理解的功能和用例组织文件。

Todo 示例：

```text
Application/Modules/Todo/
  Features/
    Tasks/
      Create/
        CreateTodoCommand.cs
        CreateTodoHandler.cs
        CreateTodoValidator.cs
      Update/
      Archive/
      Delete/
    DevelopmentPlans/
      Create/
      Update/
      Archive/
    Dependencies/
      Add/
      Remove/
      Validation/
    Roadmap/
      Queries/
      Layout/
    Search/
  Mappings/
  TodoModule.cs
```

其他模块可按下列领域组织：
- Account：Authentication、Profile、Subscriptions。
- File：Upload、Storage、Publishing、Preview、Deletion、Tagging。
- Favorite：Items、Media、Tags、Queries。
- Category：Management、Queries。
- Tag：Management、Queries、Associations。
- Corpus：Management、Queries、Random。

一个用例的 Command、Handler、Validator 尽量放在同一目录。仅当确实存在多个用例共享的实现时，才抽取到模块内的共享目录。不要为了“通用”过早把简单业务逻辑抽到 Core。

## 6. 跨模块协作与边界

### 6.1 已识别的 Account / File 双向依赖风险

此前审查的代码快照中发现：
- Account -> File：`UpdateAccountHandler` 使用文件写入、删除和发布能力；`DeleteAccountHandler` 使用文件删除能力；头像和订阅图标更新也涉及文件操作。
- File -> Account：`DeleteFileValidator` 直接依赖 `AccountSearch`，判断文件是否被账户、头像或订阅引用。

如果把这两个业务直接拆成独立项目，会形成循环 ProjectReference，必须先解耦。

建议在 `Application.Abstractions` 定义稳定的业务契约：
- `IFileAssetService`：向 Account 等调用方提供写入、发布、删除等文件资产能力。
- `IFileReferenceProvider`：由 Account、Favorite 等模块实现，用于报告文件是否仍被业务引用。
- File 模块通过注入 `IEnumerable<IFileReferenceProvider>` 汇总检查，而不是直接调用 Account 的 Search 或 Handler。

Account 和 File 都依赖 Abstractions，不互相引用实现。接口命名和方法签名应在重新审查当前 HEAD 后，依据实际调用场景确定。

### 6.2 其他已识别的数据关联

此前审查的代码快照中，统一 EF 模型存在以下业务关联：
- File ↔ Tag：FileTag 关联。
- File ↔ Category：分类映射。
- Favorite ↔ File：FavoriteMedia 外键。
- Favorite ↔ Tag：FavoriteTag 外键。
- Favorite ↔ Category：FavoriteItem.CategoryUID。
- Account ↔ Subscription。
- Todo ↔ DevelopmentPlan：PlanId。

这些是数据模型关联，不自动意味着 Application 项目之间需要直接引用。跨模块操作应通过契约、应用服务端口或明确的领域事件完成；EF Core 关系配置继续集中在 Infrastructure。

### 6.3 Mapper 的跨模块依赖

此前代码中的 FileMapper 使用通用的 `IViewMapper<TagEntity, TagDto>`、`IViewMapper<CategoryEntity, CategoryDto>` 接口。优先保留依赖抽象、由 DI 提供实现的方式，避免 File 模块直接依赖 TagMapper 或 CategoryMapper 的具体实现。

## 7. 持久化边界：先共用 DbContext，再逐步收紧端口

此前审查的代码快照中，`IApplicationDbContext` 位于 Application，并暴露 File、Corpus、Account、Subscription、Todo、DevelopmentPlan、TodoDependency、Tag、FileTag、FavoriteItem、FavoriteMedia、FavoriteTag、Category 等多个业务的 DbSet。

这会让任意业务用例都可能访问其他业务的数据。仅拆分 csproj 并不能自动建立数据访问边界。

建议分两步实施：

1. **过渡阶段**：保留一个 EF Core DbContext（当前实现为 `Infrastructure/AppStorage.cs`），保留单数据库和集中迁移；先把 DbContext 抽象从整个 Application 实现项目中移到 `Application.Abstractions`，解除 Infrastructure 对 Application 实现程序集的依赖。
2. **收敛阶段**：逐步按业务拆分持久化端口，例如 `IAccountStore`、`ITodoStore`、`IFileStore`、`ITagStore`、`ICategoryStore`、`IFavoriteStore`、`ICorpusStore`。AppStorage 可以继续实现这些接口，并在 Infrastructure 内维护统一 EF 模型和关系映射。

不要在本次重构中顺带拆数据库或迁移项目。

## 8. DI、MediatR 与组合根

此前的 `ApplicationExtensions.cs` 集中处理 Search 策略、Mapper、Validator 扫描、MediatR Handler 扫描、Pipeline Behaviors 和计划发布后台 Worker 注册。原有 Handler 扫描只扫描 `typeof(Application).Assembly`；拆分程序集后必须调整。

目标做法：
- 每个业务模块提供自己的模块注册入口，例如 `AddTodoModule()`、`AddFileModule()`，注册本模块的 Search、Mapper、Validator 和模块专属服务。
- Bootstrap 统一收集所有模块程序集，MediatR Handler 扫描覆盖全部业务模块。
- Pipeline Behaviors 只注册一次，避免重复注册。
- 确保普通 Validator 与开放泛型 Validator 都被正确注册。
- 后台 Worker 只在一个明确的组合根位置注册，避免重复启动。
- 模块注册不应依赖 Web Controller 或启动项目。

## 9. Domain 与应用层类型归属

此前代码快照中的 `Domain/Command/*.cs` 包含 AccountCommand、FileCommand、TodoCommand 等应用用例请求。它们应移动到各自的 Application 模块，因为 Command / Query 是应用用例输入，不是领域实体或领域规则。

类似 `IRequireValidEntity<T>`、`IRequireDevPlan` 等用于应用 Pipeline 的标记接口，应移动到 `Application.Abstractions` 或适当的应用层公共项目。

待应用层引用全部迁出后，再评估移除 Domain 对 MediatR 的依赖。不要在迁移未完成时一次性删除现有依赖。

## 10. 推荐实施顺序

### 阶段 0：冻结基线与复核

- 确认实际工作分支和最新 HEAD；本文创建时以 `dev` 分支 HEAD 为重构分支基线。
- 建立独立重构分支，保留原 README 和其他现有文件。
- 在重构开始前运行 `dotnet build`、`dotnet test`，记录结果；如可用，再运行仓库 CI。
- 记录现有项目引用图、DI 注册、MediatR 扫描范围和 API 行为。
- 重新核对此前审查快照中识别的依赖关系，避免把旧 commit 的发现直接当作当前 HEAD 的事实。
- 现有 P0 业务缺陷单独登记、单独修复，不与纯结构移动混在同一个提交。

### 阶段 1：建立 Abstractions 与 Core

- 新建 `Application.Abstractions` 和 `Application.Core`。
- 迁移共享接口、Pipeline 标记接口、Search 基础能力、Mapper 接口和持久化契约。
- 优先移动 `IApplicationDbContext`，使 Infrastructure 不再依赖整个 Application 实现项目。
- 暂时保持原有行为和注册方式，确保解决方案可编译。

### 阶段 2：Todo 作为试点模块

- 新建 `AqLife.Application.Todo`。
- 迁移 Todo Commands / Queries、Handlers、Validators、Search、Mappings、Roadmap、Dependencies 等。
- 按 Feature Folder 重新组织。
- 补充或迁移单元测试，验证行为一致。
- Todo 已知业务问题应在独立提交中修复，避免结构变更和行为变更难以区分。

### 阶段 3：先打通跨模块契约

- 优先解除 Account / File 双向实现依赖。
- 定义文件资产操作和文件引用检查契约。
- 梳理 File、Tag、Category、Favorite 的交叉引用，确定接口所有者和调用方向。
- 在解耦完成前，不创建相互引用的业务项目。

### 阶段 4：逐步抽取其他业务模块

建议顺序：
1. Corpus。
2. Tag 与 Category（结合关联关系一起设计）。
3. File（在引用检查和文件资产契约稳定后）。
4. Account（在 File 抽象稳定后）。
5. Favorite（在其依赖的文件、标签、分类契约稳定后）。

若当前代码依赖审查发现更合适的顺序，可以调整，但必须维持无业务模块循环引用。

### 阶段 5：调整 DI 与 MediatR

- 由每个模块注册自己的模块服务。
- Bootstrap 收集所有模块程序集。
- MediatR Handler 扫描覆盖所有模块。
- Pipeline Behaviors 只注册一次。
- 验证 Validator、Mapper、Search 和 Hosted Worker 均只注册且生效一次。

### 阶段 6：收敛持久化端口

- 将全局 DbContext 接口逐步拆成按业务职责划分的 Store / Repository / Query Port。
- AppStorage 继续实现各模块端口。
- 继续使用单数据库、单 DbContext 实现和集中 migrations。
- 检查每个模块只通过自己的端口访问数据，跨模块查询通过显式契约完成。

### 阶段 7：清理旧项目

- Web、Bootstrap、Tests 等所有引用切换到新模块。
- 删除旧 Application 项目之前，确认没有残余引用和遗漏类型。
- 更新解决方案文件、CI、脚本、文档和测试项目引用。
- 删除旧项目并进行完整回归。

## 11. 测试与验收标准

### 11.1 构建与行为

- 每个 Application 模块可独立编译。
- 完整 API 可构建、启动，现有 HTTP 契约和业务行为保持一致。
- 单元测试、集成测试和仓库 CI 通过；无法通过的基线问题必须有明确记录。
- 模块移动前后关键用例有回归测试。

### 11.2 依赖边界

- 业务模块之间没有直接 ProjectReference。
- Account 与 File 不存在循环依赖。
- Infrastructure 不引用 Application 业务实现项目。
- Bootstrap 是模块实现的集中组合根。
- 跨模块调用只依赖稳定契约，不依赖对方 Handler、Search 或 Mapper 的具体实现。

### 11.3 运行时注册

- MediatR 能发现所有模块 Handler。
- Pipeline Behavior 不重复注册。
- 普通与开放泛型 Validator 均能解析。
- Search、Mapper、模块服务和后台 Worker 注册完整且无重复副作用。

### 11.4 持久化与部署

- 保留一个 API 入口和一个部署单元。
- 保留单一 PostgreSQL 数据库、统一 EF Core DbContext 实现和集中 migrations。
- 模块数据访问逐步通过各自端口收敛。
- 不引入服务间网络通信或分布式一致性机制。

### 11.5 架构测试

建议新增 ArchitectureTests，自动验证：
- Account 不引用 File，File 不引用 Account。
- 任意业务模块不引用其他业务模块实现项目。
- Infrastructure 不引用 Application.Account、Application.Todo 等业务实现程序集。
- Web 不承载业务实现逻辑。
- 模块对外依赖只通过 Abstractions 暴露的契约。

## 12. 风险与执行纪律

- **基线漂移**：重构开始前重新确认分支 HEAD；此前静态审查来自历史代码快照，必须复核。
- **行为变化**：结构移动与业务修复分开提交，便于定位回归。
- **DI 遗漏**：拆程序集后最容易出现 Handler、Validator、Mapper 未扫描或 Worker 重复注册。
- **循环依赖**：不能以新增 ProjectReference 临时解决跨模块访问，应先设计契约。
- **数据边界虚假**：若所有模块仍能访问全局 DbSet，项目边界只是编译边界，必须逐步收敛持久化端口。
- **过度抽象**：只把真正跨模块、稳定且有多个调用方的契约放入 Abstractions；业务专属逻辑留在模块内部。
- **迁移粒度**：一次只迁移一个模块或一组紧密关联的模块，每一步都保持可构建、可测试、可回滚。

## 13. 当前代码审查发现的追踪清单

以下发现来自此前对 `services/API` 历史代码快照的静态审查，作为复核清单使用，不应未经核对就认定它们仍完全对应当前分支：

- `IApplicationDbContext` 位于 Application 且暴露多个业务 DbSet，Infrastructure 因实现该接口而引用 Application。
- Account 与 File 存在双向业务实现依赖风险。
- File、Tag、Category、Favorite 存在数据关联，需要通过契约定义模块协作。
- `ApplicationExtensions.cs` 承担集中式注册，原扫描逻辑只扫描单一 Application 程序集。
- Domain/Command 下存在应用用例请求类型。
- FileMapper 使用 Tag / Category 的通用 Mapper 抽象，宜保持接口依赖而非具体 Mapper 依赖。

复核时应记录每项发现对应的当前文件、类型、调用方、目标契约、测试覆盖和处理状态。

## 14. 推荐提交策略

建议按以下逻辑拆分提交，避免一个巨大重构提交：
1. `docs: add modular monolith refactoring guide`
2. `refactor(api): introduce application abstractions and core`
3. `refactor(todo): extract todo application module`
4. `refactor(api): decouple account and file contracts`
5. `refactor(api): extract remaining application modules`
6. `refactor(api): centralize module registration`
7. `refactor(api): enforce module persistence boundaries`
8. `test(api): add architecture boundary tests`

---

**最终架构定位：AqLife 是一个采用模块化单体架构的 Monorepo。业务 Application 项目独立编译、边界清晰，但仍由同一个 API 进程统一加载和部署；数据库、EF Core DbContext 实现及 migrations 在当前阶段保持集中。**
