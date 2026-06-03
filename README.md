# MyLife

个人站点：ASP.NET Core 8 API + Vue 3（Vite）**SPA** 前端。

> **方向说明**：已取消 SSR 方案（不引入 Nuxt / Express SSR 等）。博客与文档预览在浏览器内完成 Markdown 解析与渲染即可。

---

## 前端目录结构评估（`Web/src/`）

### 当前结构（摘要）

```
Web/src/
├── api/generated/       # OpenAPI 生成的 TS 客户端
├── assets/
│   ├── styles/main.css
│   ├── icons/           # 导航等 SVG 组件
│   └── fonts/
├── components/          # 通用 UI（含 Markdown 相关半成品）
├── composables/         # 组合式逻辑（如 useNavigation）
├── data/                # 静态展示数据（技能、开发计划等）
├── services/
│   ├── router/          # routerRaw + createAppRouter
│   └── storer.ts        # Pinia store + Api Configuration
├── Skeletons/           # 加载占位
├── types/
├── utils/
├── views/               # 路由页面
├── App.vue
└── shims-vue.d.ts
```

### 合理之处

| 点 | 说明 |
|----|------|
| `views` / `components` 分离 | 页面与可复用 UI 边界清晰 |
| `services/router/` | 路由表与 `createRouter` 拆分，便于维护 |
| `api/generated` | 与后端契约一致，类型安全 |
| `composables` + `types` | 导航等横切逻辑可复用 |
| `Skeletons/` | 与真实内容组件分离，利于加载态 |

### 建议调整（待办）

- [X] **恢复 SPA 唯一入口 `main.ts`**：`index.html` 已引用 `/src/main.ts`，但文件缺失；`entry-client.ts` / `app.ts` 内容不完整，无法启动
- [X] **清理 SSR 残留文件**：删除 `entry-server.ts`；删除或合并 `app.ts`、`entry-client.ts`
- [X] **路由仅保留 SPA 模式**：`routerBuild.ts` 去掉 `createMemoryHistory` / `import.meta.env.SSR` 分支，固定 `createWebHistory()`
- [ ] **收敛 `services/storer.ts` 职责**：Pinia 与 `ApiOption` 可拆为 `stores/` + `api/client.ts`（可选，降低单文件臃肿）
- [ ] **`MarkdownViewer.vue` 命名与职责**：当前是「文章壳 + `v-html`」，并非解析器；重命名或拆为 `BlogArticleLayout.vue` + 真正的 `MarkdownRenderer`
- [ ] **`api/generated/docs/`**：生成器文档目录可不进仓库或加入 ignore（减小体积）
- [ ] **统一资源路径**：样式已在 `assets/styles/main.css`，确保各入口只引用一处全局 CSS

### 可选的演进结构（非必须）

若博客功能变多，可增加按功能划分（二选一即可）：

```
src/features/blog/
  components/   # MarkdownRenderer、TOC、CodeBlock
  composables/  # useMarkdown、useBlogArticle
  stores/       # useBlogStore
```

当前规模下维持 `components/` + `composables/` 也足够，不必过早拆分。

---

## SPA 收尾待办

- [X] 新建 `src/main.ts`：`createApp(App)` + Pinia + Router + Element Plus + `mount('#app')`（使用 `createApp`，非 `createSSRApp`）
- [X] 删除 `entry-server.ts`、`entry-client.ts`、`app.ts`（或仅保留已被 `main.ts` 替代后的空引用清理）
- [ ] 确认 `vite dev` / `vite build` 可正常运行
- [ ] `storer.ts`：`VITE_API_BASE_URL` 替代硬编码 `http://localhost:5110`
- [ ] `utils/request.ts`：统一错误提示；后续接入 JWT 时在此注入 `Authorization`
- [ ] `CodeBlock.vue`：补全 `ElMessage` 导入、复制按钮 `@click` 绑定（当前逻辑未挂载到模板）

---

## Markdown 渲染器 — 设计逻辑（SPA）

### 目标

在 **建文 / 博客** 场景下：从后端 `FileApi` 获取 `.md` 原文 → 浏览器内解析为安全 HTML → 代码高亮、标题锚点、Mermaid 图表、目录（TOC）、代码块复制。

### 分层架构

```
┌─────────────────────────────────────────────────────────┐
│  views/BlogView、文章详情路由                             │
└───────────────────────────┬─────────────────────────────┘
                            │ 拉取 md 文本、元数据
┌───────────────────────────▼─────────────────────────────┐
│  Pinia blog store（或 composable）                        │
│  - 当前文章 id / title / rawMarkdown / toc[]             │
└───────────────────────────┬─────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────┐
│  composables/useMarkdown.ts                               │
│  - render(markdown: string) => { html, headings }         │
│  - 封装解析、TOC 抽取、缓存策略                             │
└───────────────────────────┬─────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────┐
│  lib/markdown/renderer.ts（新建）                         │
│  - 单一引擎：markdown-it（弃用 marked / vue-markdown-render）│
│  - 插件：anchor、highlightjs、自定义 fence（代码块）         │
└───────────────────────────┬─────────────────────────────┘
                            │ html string
┌───────────────────────────▼─────────────────────────────┐
│  components/markdown/MarkdownRenderer.vue                 │
│  - DOMPurify 消毒后 v-html                                 │
│  - onMounted：mermaid.run() 处理 ```mermaid 块             │
└───────────────────────────┬─────────────────────────────┘
         ┌──────────────────┼──────────────────┐
         ▼                  ▼                  ▼
   CodeBlock.vue        TOC.vue          文章布局壳组件
```

### 数据流

1. **列表 / 目录**：`FileApi` → 构建树形结构（`BlogView` 中 `ElTree`，字段与后端 `FileMeta` 对齐）。
2. **打开文章**：按文件 id 请求内容（下载或专用正文接口）→ 得到 **原始 Markdown 字符串**。
3. **解析**：`useMarkdown.render(raw)` → `{ html, headings }`。
4. **展示**：`MarkdownRenderer` 输出正文；`TOC` 绑定 `headings`（`h1`–`h3` 等，带 `id` 锚点）。
5. **增强（仅浏览器）**：
   - 普通代码块：`markdown-it-highlightjs`；
   - Mermaid：解析后 DOM 上 `querySelectorAll('.mermaid')` + `mermaid.run()`（放在 `onMounted`，避免阻塞首屏可先占位）；
   - 复制：`CodeBlock` 从 DOM 或 fence 元数据取文本，`navigator.clipboard`。

### 模块职责

| 模块 | 职责 | 不应负责 |
|------|------|----------|
| `lib/markdown/renderer.ts` | 配置 markdown-it、插件、自定义 renderer（fence → 带 `data-lang` 的 `<pre>`） | 不发 HTTP、不持路由状态 |
| `useMarkdown.ts` | 调用 renderer、解析 TOC、可选 LRU 缓存（path → html） | 不直接操作 Element Plus |
| `MarkdownRenderer.vue` | 消毒 + `v-html` + 触发 Mermaid | 不内嵌巨型解析配置 |
| `CodeBlock.vue` | 展示 + 复制交互 | 不做 Markdown 解析 |
| `TOC.vue` | 根据 `headings[]` 渲染 `ElAnchor` | 不重复解析全文 |
| Pinia `Blog` store | 当前文章、目录树、加载态、缓存列表 | 不替代 renderer 纯函数 |

### 安全

- [ ] 所有 `v-html` 前经 **DOMPurify**（或 markdown-it 白名单标签配置），防止 XSS。
- [ ] 不信任用户上传 MD 中的原始 HTML（禁用或剥离 `html: true`）。
- [ ] 外链 `target="_blank"` + `rel="noopener noreferrer"`（可在 markdown-it 链接插件中统一处理）。

### 依赖收敛

| 保留 | 移除 / 避免 |
|------|-------------|
| `markdown-it` + `markdown-it-anchor` + `markdown-it-highlightjs` | `marked`（二选一） |
| `mermaid`（客户端初始化） | `markdown-it-mermaid`（若与 Vue 生命周期冲突可不用，改 hands-on `mermaid.run`） |
| 可选 `dompurify` + `@types/dompurify` | `vue-markdown-render`（与自研管线重复） |

### 与后端协作

- 上传：已有 `UpLoadFileCard.vue` 面向 `.md`，需对接 `FileApi` 上传与策略（`FilePolicy` 扩展名）。
- 存储：正文在服务端；前端只缓存 **已解析结果** 或 **raw**，按体积择一。
- 路由建议：`/blog` 列表 → `/blog/:fileId` 阅读页（阅读页挂载 `MarkdownRenderer`）。

---

## Markdown 渲染器 — 实施待办

### 阶段 A：基础管线

- [ ] 新建 `src/lib/markdown/renderer.ts`，统一 `markdown-it` 实例与插件注册
- [ ] 新建 `src/composables/useMarkdown.ts`，暴露 `render()` 与 `extractHeadings()`
- [ ] 安装并接入 `dompurify`；`MarkdownRenderer.vue` 只负责展示层
- [ ] 从 `package.json` 移除未使用的 `marked`、`vue-markdown-render`（完成迁移后）

### 阶段 B：代码块与 TOC

- [ ] 自定义 fence：代码块输出结构与 `CodeBlock.vue` 样式一致，或解析后用 Vue 替换（对应 DevPlan「JSX → Vue 组件」）
- [ ] 修复 `CodeBlock.vue`：复制按钮、依赖导入、从块内读取代码文本
- [ ] 完善 `TOC.vue`：消费 `headings`，绑定 `ElAnchorLink`（`href="#id"`）
- [ ] 扩展 Pinia `Blog` store：`blogTitle`、`raw`、`html`、`headings`、加载/错误态

### 阶段 C：Mermaid 与博客页

- [ ] 在 `MarkdownRenderer` 的 `onMounted` / `watch` 中初始化 Mermaid（主题与站点 CSS 变量协调）
- [ ] `BlogView`：列表对接 `FileApi`；新增文章详情路由与阅读布局
- [ ] `UpLoadFileCard.vue`：对接上传 API、限制 `.md`、上传成功后刷新目录树
- [ ] 样式：`#blogContent` 内 typography（标题、列表、引用、表格）在 `assets/styles` 中统一

### 阶段 D：体验与缓存

- [ ] 已访问文章内存缓存（store 或 `Map<fileId, RenderResult>`）
- [ ] 解析错误兜底 UI（非白屏）
- [ ] 大文档：可选 `requestIdleCallback` 再跑 Mermaid，避免卡顿

---

## 仓库工程化（通用）

- [x] 修正 `.gitignore`：避免误忽略 `package.json`、`README.md` 等
- [x] 敏感配置：`appsettings*.json` 不提交，使用 `*.example.json` 模板
- [X] 添加 `MyLife.sln`，便于 VS / Rider / CI
- [x] EF `Migrations` 是否入库及协作流程文档化
- [x] 配置本地开发：复制 `appsettings.Development.example.json` → `appsettings.Development.json` 并填入本地数据库（勿提交）

---

## 建议实施顺序

1. SPA 入口恢复 + 删除 SSR 残留（先能 `npm run dev`）
2. 环境变量与 API 客户端
3. Markdown 阶段 A → B（能渲染一篇本地 md 或接口 md）
4. 博客路由 + FileApi + 上传
5. Mermaid 与缓存优化

---

## 后端 File 与 FileStrategy 问题清单（待修复 / 待确认）

> 以下为代码评审结论，**追加**记录；修复前请确认 `FileName` 等字段的**业务语义**（是否含扩展名）及对已有数据的影响。

### 策略注册与上传路径

- [x] **`FileService.HandleUploadAsync` 对 `IEnumerable<IFileExtStrategy>` 全量 `foreach`**：当前同时注册了 `UploadPermissionCheck` 与 `DownloadPermissionCheck`，上传时会对同一扩展名执行**两套**校验；若 `AllowedUpload` 与 `AllowedDownload` 不一致，会出现「允许上传的类型却上传失败」等非预期行为。建议上传只跑「上传相关」策略（拆分接口或分组注册）。
- [x] **`IFileMetaStrategy`（`UploadSizeCheck`）已注入但未调用**：`FileService` 构造函数接收 `_fileMetaStrategy`，`HandleUploadAsync` 中未执行 `_fileMetaStrategy.Check(file)`，文件大小限制**未生效**。应在保存前显式调用，或改为 `IEnumerable<IFileMetaStrategy>` 与其它 meta 策略统一遍历。

### `FileSearch` 与扩展名校验

- [x] **匿名列表/单条查询中的 `_uploadCheck.Check(e.DesensitizationName)`**：`UploadPermissionCheck.Check(string ext)` 语义为**扩展名**（如 `.md`），传入整段 `DesensitizationName` 易与配置不匹配。应改为 `Path.GetExtension(e.DesensitizationName)`（或与存储命名规则一致的字段），并核对 `AllowedUpload` / 匿名可见策略是否应用「上传白名单」语义。

### 搜索策略优先级

- [ ] **`FindFileAsync` 使用 `FirstOrDefault(s => s.IsMatch(...))`**：若调用方同时提供 `title` 与 `id`，命中策略取决于 DI 注册顺序，**不确定**。建议约定优先级（通常 **id 优先于 title**），或在 `IsMatch` 中互斥。

### `FilePolicyFilter` 与配置解析

- [x] **`GetRequiredService<FilePolicyOption>()`**：通常仅 `AddOptions<FilePolicyOption>().Bind(...)` 时，**不会**将 `FilePolicyOption` 注册为可直接 `GetRequiredService<T>()` 的具体类型；更稳妥为 `GetRequiredService<IOptions<FilePolicyOption>>().Value` 或注入 `IOptionsSnapshot<FilePolicyOption>`。需在实际上传接口上验证过滤器是否稳定解析配置。

### 实体与重复检测

- [x] **`FileMetaEntity` 构造函数中 `DesensitizationName`**：`FileName` 已使用 `GetFileNameWithoutExtension`，随后 `Path.GetExtension(FileName)` 几乎恒为空，导致磁盘文件名**丢失真实扩展名**。应使用 `Path.GetExtension(file.FileName)`（或与 `FileName` 设计一致的后缀来源）。
- [x] **`UploadFileEffectivenessCheck` 中 `e.FileName == file.FileName`**：实体侧 `FileName` 为无扩展名存储，上传侧 `IFormFile.FileName` 常含扩展名，**重复检测易失效**。应对齐比较字段（例如统一比较「无扩展名」或统一「全名」），并明确「重复」定义（同哈希 / 同名 / 二者组合）。

### 下载与其它

- [x] **`GetFileInternalAsync` 中下载扩展名校验**：通过 LINQ 在 `_strategies` 中筛 `DownloadPermissionCheck`，可读性一般；可改为直接依赖 `DownloadPermissionCheck` 或单独抽象，避免与上传策略混在同一集合的误用。
- [x] **修复前注意**：若调整 `FileName` / `DesensitizationName` 规则，需评估 **已有库表与磁盘文件** 的迁移或兼容策略。

## 需求分析

后端需求拆分视图

```mermaid
requirementDiagram

direction TB

designConstraint API{
   id:6
   text:"所有 WEB API 的需求约束"
}

designConstraint API_Service{
   id:4
   text:"所有 WEB API 的底层服务实现"
}

designConstraint App_Service{
   id:2
}

functionalRequirement Safe{
   id:1
   text:"业务安全"
}

designConstraint Data_Design{
   id:3
}


element Account{
   type:Entity
}

element FileMeta{
   type:Entity
}

element Todo{
   type:Entity
}

element "Initialization check"{
   type:App_Service
}

element "Runtime check"{
   type:App_Service
}


Account - satisfies -> Data_Design
FileMeta - satisfies -> Data_Design
Todo - satisfies -> Data_Design

"Initialization check" - refines -> Safe
"Runtime check" - refines -> Safe
API_Service - refines -> API

Safe - verifies -> API
Safe - verifies -> App_Service
Safe - verifies -> Data_Design
```

