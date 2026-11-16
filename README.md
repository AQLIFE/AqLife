# AqLife

AqLife 是一个基于 **.NET 8、Vue 3 和 PostgreSQL** 的 Monorepo 项目，包含后端 API、公开 Web 前端和管理端 MGS。

## 项目组成

| 模块 | 技术 | 职责 |
|---|---|---|
| API | ASP.NET Core 8、EF Core、MediatR | 业务接口、身份认证、数据访问与文件服务 |
| Web | Vue 3、TypeScript、Vite | 面向访客的公开网站与只读内容浏览 |
| MGS | Vue 3、TypeScript、Vite、Element Plus | 管理端，维护内容、账户及开发计划 |
| Database | PostgreSQL 18 | 持久化业务数据 |
| Monorepo | npm Workspaces | 管理前端应用、共享包和工程脚本 |

### 当前主要能力

- **内容与文件**：文件上传、下载、内容发布及文件分类/标签管理。
- **账户**：账户注册、认证及账户信息管理。
- **收藏**：收藏内容及其分类、标签管理。
- **开发计划**：多计划管理、任务层级、任务状态与归档。
- **任务依赖与科技树**：维护任务前置依赖，计算阻塞状态，并保存科技树节点布局。
- **前后端契约**：通过 OpenAPI 生成 TypeScript API 契约和客户端，减少手写接口定义造成的不一致。

MGS 负责管理操作；Web 面向访客提供公开内容和开发计划浏览。具体权限以 API 的认证与授权规则为准，不能仅依赖前端隐藏操作入口。

## 仓库结构

```text
AqLife/
├── apps/
│   ├── Web/                    # 公开端 Vue 应用
│   └── MGS/                    # 管理端 Vue 应用
├── services/
│   └── API/
│       ├── Web/                # ASP.NET Core 启动项目、Controllers、配置
│       ├── Application/        # 应用服务、Handlers、查询与业务流程
│       ├── Domain/             # 领域实体与领域规则
│       ├── Infrastructure/     # EF Core、文件存储等基础设施
│       ├── Shared/             # 跨层共享类型
│       ├── Extensions/         # DI 与框架扩展
│       ├── PostgreSqlMigrations/ # PostgreSQL EF Core migrations
│       └── CoreTest/           # 单元测试与集成测试
├── packages/
│   ├── api-contract/           # OpenAPI 生成的 API 类型契约
│   ├── api-client/             # 前端 API 客户端
│   ├── domain/                 # 前端共享领域类型与逻辑
│   ├── icons/                  # 共享图标
│   └── ui-shared/              # 共享 UI 能力
├── tools/
│   ├── ci/                     # CI 检查脚本
│   ├── openapi-generator/      # API 契约生成工具
│   └── scripts/                # 开发、启动及维护脚本
├── config/                     # 共享工程配置
├── package.json
└── README.md
```

## 环境要求

- Node.js 24+
- .NET SDK：仓库 `global.json` 指定 10.0.401；API 项目目标框架为 .NET 8
- PostgreSQL 18
- npm

安装前端依赖：

```bash
npm install
```

## 本地配置

### 1. 创建 PostgreSQL 数据库

例如：

```sql
CREATE DATABASE aqlife;
```

请按实际部署情况创建数据库用户并授予访问权限。

### 2. 配置 API

复制开发配置模板：

```bash
cp services/API/Web/Configurations/appsettings.Development.example.json services/API/Web/Configurations/appsettings.Development.json
```

编辑本地配置，确保数据库配置使用当前项目支持的 PostgreSQL。核心配置示例：

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=127.0.0.1;Port=5432;Database=aqlife;Username=YOUR_USER;Password=YOUR_PASSWORD"
  },
  "DbOption": {
    "DbType": "PostgreSQL",
    "DbVersion": "18",
    "MigrationAssembly": "PostgreSqlMigrations"
  }
}
```

将该配置合并到 `appsettings.Development.json`，并按需保留模板中的 JWT、CORS 和日志设置。不要提交本地配置、数据库密码、JWT 密钥或生产环境 Secret。

> 注意：当前仓库中的 `appsettings.Development.example.json` 仍包含旧的 MYSQL 示例字段。项目当前数据库实现只接受 `DbType=PostgreSQL`，请按上面的配置调整，不要直接使用模板中的 MYSQL 值。

API 文件存储使用 Cloudflare R2。首次运行 API 前，需要在当前电脑为该 API 项目配置以下四项 .NET User Secrets：

```bash
dotnet user-secrets set "R2:SecretKey" "YOUR_R2_SECRET_KEY" --project services/API/Web/Web.csproj
dotnet user-secrets set "R2:Endpoint" "YOUR_R2_ENDPOINT" --project services/API/Web/Web.csproj
dotnet user-secrets set "R2:BucketName" "YOUR_R2_BUCKET_NAME" --project services/API/Web/Web.csproj
dotnet user-secrets set "R2:AccessKey" "YOUR_R2_ACCESS_KEY" --project services/API/Web/Web.csproj
```

请替换为 Cloudflare R2 控制台提供的实际值。User Secrets 保存在本机用户配置目录中，不会提交到 Git；每台需要运行 API 的电脑都必须单独设置。不要将密钥写入 `appsettings*.json` 或提交到仓库。

### 3. 配置 Web 和 MGS 环境变量

Web 和 MGS 共用仓库根目录下的 `config/env` 环境配置目录（由 Vite 的 `envDir` 指定）。首次运行任一前端前，先从模板创建两个环境文件：

```bash
cp config/env/.env.example config/env/.env.dev
cp config/env/.env.example config/env/.env.prod
```

Windows PowerShell 也可以使用：

```powershell
Copy-Item config/env/.env.example config/env/.env.dev
Copy-Item config/env/.env.example config/env/.env.prod
```

模板内容如下：

```dotenv
# .env.development
VITE_API=http://192.168.0.100:5110
```

根据当前电脑的 API 地址修改 `VITE_API`。开发模式读取 `.env.dev`，Production 模式读取 `.env.prod`；两个前端应用共用这两个文件。若生产 API 使用 HTTPS 或不同域名/端口，请相应修改 `.env.prod` 中的地址，并确保前端与 API 协议一致。

Git 会跟踪 `config/env/.env.example`，但忽略本机的 `.env.dev` 和 `.env.prod`。环境文件应在每台运行前端的电脑上分别创建，不要将本机环境配置提交到仓库。

### 4. 更新数据库结构

在仓库根目录执行：

```bash
dotnet ef database update \
  --project services/API/PostgreSqlMigrations/PostgreSqlMigrations.csproj \
  --startup-project services/API/Web/Web.csproj
```

当前 PostgreSQL 迁移链以 `InitialNewBaseLinePostgreSQL` 为基线。基线之前的旧迁移不属于当前有效迁移链；新迁移应从当前基线继续生成。

已有数据库切换到新基线时，应先核对数据库结构和迁移历史，再按受控流程同步迁移记录，不要直接对已有数据重复执行初始化建表。

## 启动服务

在仓库根目录运行。每个服务都可以独立启动，不要求三个服务同时运行。

### 开发模式（Development + HTTP）

```bash
npm run dev:api
npm run dev:web
npm run dev:mgs
```

也可以直接运行 API：

```bash
dotnet run --project services/API/Web/Web.csproj
```

### Production 模式（HTTPS）

```bash
npm run prod:api
npm run prod:web
npm run prod:mgs
```

启动脚本会明确设置环境，不依赖终端中遗留的 `APP_ENV` 值。当前只支持 `dev` 和 `prod` 两种模式。

| 服务 | 开发模式 | Production 模式 |
|---|---|---|
| API | `http://localhost:5110` | `https://localhost:7110` |
| Web | `http://localhost:5173` | `https://localhost:5173` |
| MGS | `http://localhost:5200` | `https://localhost:5200` |

API Swagger 仅在 Development 环境开放：`http://localhost:5110/swagger`。

前端默认根据当前页面的协议和主机名推导 API 地址：HTTP 对应 API 5110，HTTPS 对应 API 7110。需要覆盖默认地址时可设置 `VITE_API`，并确保页面与 API 协议一致。

## HTTPS 与局域网访问

Production 启动脚本使用 HTTPS：

- Web 和 MGS 使用 PEM 证书。
- API 使用 PFX 证书，默认路径为仓库根目录 `.https/server.pfx`。
- 可使用 `mkcert` 为本地开发电脑及局域网地址签发证书，再用 OpenSSL 导出 PFX。

示例（在仓库根目录执行，将 IP 替换为实际开发电脑的局域网地址）：

```bash
mkdir -p .https
mkcert -install
mkcert \
  -cert-file .https/server.pem \
  -key-file .https/server-key.pem \
  localhost 127.0.0.1 ::1 192.168.0.100

openssl pkcs12 -export \
  -out .https/server.pfx \
  -inkey .https/server-key.pem \
  -in .https/server.pem
```

API 的 PFX 密码可通过 .NET User Secrets 配置：

```bash
dotnet user-secrets set "Kestrel:Certificates:Default:Path" "$(pwd)/.https/server.pfx" --project services/API/Web
dotnet user-secrets set "Kestrel:Certificates:Default:Password" "YOUR_PFX_PASSWORD" --project services/API/Web
```

也可使用环境变量 `ASPNETCORE_Kestrel__Certificates__Default__Path` 和 `ASPNETCORE_Kestrel__Certificates__Default__Password`。证书文件和密钥不应提交到仓库；mkcert 的 `rootCA-key.pem` 尤其不能分享。其他局域网设备需要单独信任签发证书的 CA 根证书。

API 的 CORS 来源通过 `Cors:AllowedOrigins` 配置。Development 环境必须配置允许来源。局域网访问时，应将前端实际访问的 IP/域名及端口加入 CORS 配置，并在防火墙中开放对应端口。mkcert 证书仅用于本地或可信局域网测试；公网生产环境应使用可信 CA 证书或由反向代理/托管平台终止 TLS。

## 常用工程命令

| 命令 | 用途 |
|---|---|
| `npm run bootstrap` | 初始化/检查 Monorepo 工程依赖 |
| `npm run generate:contract` | 生成前端 API 契约 |
| `npm run ci` | 执行完整 CI 检查 |
| `npm run ci:frontend` | 执行前端检查 |
| `npm run ci:api` | 执行 API 检查 |

前端也可以在各自 workspace 中执行 `type-check`、`build` 等脚本。具体脚本以对应应用的 `package.json` 为准。

## 架构约定

- **后端分层**：Domain 管理领域模型与规则；Application 编排用例；Infrastructure 实现数据库和文件存储；Web 提供 HTTP API。
- **数据库迁移**：使用 EF Core 与 PostgreSQL migrations 项目管理数据库结构变更。
- **前端契约**：后端 OpenAPI 定义作为 API 契约来源；生成结果维护在 `packages/api-contract`，客户端封装在 `packages/api-client`。
- **共享代码**：前端应用通过 npm Workspaces 使用 `packages/*`，避免重复维护接口类型和通用能力。
- **质量检查**：`services/API/CoreTest` 包含后端单元测试与集成测试；仓库 CI 脚本负责执行工程检查。

## 贡献与变更

修改 API 契约或 DTO 后，检查并按需重新生成前端契约；修改 EF Core 实体或模型配置后，评估并生成新的数据库迁移。提交前建议运行与改动范围对应的 CI 检查。
