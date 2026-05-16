# MyLife

个人站点项目：ASP.NET Core 8 API + Vue 3（Vite）前端。当前前端为 SPA；以下为 **SSR 改造与后续开发** 待办清单。

---

## SSR 改造待办

### 阶段一：工程骨架

- [ ] 新建 `src/app.ts`：`createApp()` 工厂，每次请求创建独立的 `app` / `pinia` / `router`（禁止模块顶层单例）
- [ ] 拆分入口
  - [ ] `src/entry-client.ts`：`createSSRApp` + `app.mount('#app', true)` 做 **hydrate**
  - [ ] `src/entry-server.ts`：`renderToString`，输出 HTML 片段与预加载状态
- [ ] 改造 `index.html` 为模板：预留 `<!--ssr-outlet-->`、`<!--pinia-state-->` 等占位
- [ ] 新增 Node 入口 `server.ts`：静态资源 + SSR 渲染（开发 / 生产）
- [ ] 更新 `package.json` 脚本：`dev:ssr`、`build:ssr`、`preview:ssr`（或等价命名）
- [ ] 更新 `vite.config.ts`
  - [ ] SSR build（`build.ssr` 或 `vite build --ssr`）
  - [ ] `ssr.noExternal` 包含 `element-plus` 等需在 Node 内打包的依赖

### 阶段二：路由与环境

- [ ] 路由双模式：服务端 `createMemoryHistory()`，客户端 `createWebHistory()`
- [ ] `entry-server` 中：`router.push(url)` → `await router.isReady()` → 再 `renderToString`
- [ ] 环境变量分层
  - [ ] 浏览器：`VITE_API_BASE_URL`
  - [ ] Node SSR：`API_SERVER_URL`（内网访问 API，勿与 `VITE_` 混用）
- [ ] 重构 `src/services/storer.ts`：去掉 `http://localhost:5110` 硬编码，统一 `Configuration` 工厂

### 阶段三：数据与状态（注水）

- [ ] Pinia 状态序列化：服务端写入 `window.__PINIA_STATE__`，客户端 hydrate 前恢复
- [ ] 首屏数据改到 SSR 可执行路径（在 `renderToString` 之前完成）：
  - [ ] `HomeView`：`apiCorpusRandomGet`（注意随机内容可能导致 hydration 不一致）
  - [ ] `FAuthorCard` / `AuthorCard`：`apiAccountGet`
  - [ ] 优先使用：路由 `meta.loader` / `beforeEnter`，或组件 `async setup` + `await`
- [ ] 逐步减少仅依赖 `onBeforeMount` 的首屏请求

### 阶段四：UI 与仅客户端能力

- [ ] Element Plus SSR：确保首屏 CSS 完整，查阅官方 SSR 指南
- [ ] 新增 `ClientOnly` 组件（或等价方案），包裹仅浏览器逻辑：
  - [ ] `CodeBlock.vue`：`navigator.clipboard`
  - [ ] `SearchBox.vue`：`window.addEventListener`
  - [ ] `HashData.ts`：`window.crypto`（SSR 侧改用服务端 crypto 或仅客户端生成）
- [ ] Markdown：服务端 `markdown-it` 转 HTML；**mermaid** 客户端再渲染（占位 + hydrate 后初始化）
- [ ] 收敛 Markdown 依赖（`markdown-it` / `marked` 二选一）

### 阶段五：SEO、安全与部署

- [ ] 接入 `@unhead/vue`（或同类）：每页 `title` / `meta`，SSR 时写入 `<head>`
- [ ] JWT：SSR 请求 API 时转发 `Cookie` / `Authorization`；**勿**把 token 写入注水 JSON
- [ ] 服务端 render 失败时返回兜底 HTML（避免白屏）
- [ ] 生产部署：Node 托管 SSR + 静态资源，或由 ASP.NET 反代到 Node SSR 进程
- [ ] 验收：构建后「查看网页源代码」首屏有正文；关键页关闭 JS 仍可读核心内容

### 阶段六：渐进范围

- [ ] 明确哪些路由走 SSR、哪些保持 CSR（例如 `/blog/:id`、`/about` SSR；`/option` CSR）
- [ ] 博客正文与关于页优先 SSR；配置页可延后

---

## 未来开发注意事项

### 应在 SSR 环境可运行

- `async setup` 中的 `await` 请求
- 纯计算、props、Pinia getter
- `import.meta.glob`（保持路径稳定）

### 必须放在客户端

- `window` / `document` / `localStorage` / `navigator`
- mermaid、剪贴板、全局快捷键
- 强依赖 DOM 的 Element Plus API（如 `ElMessage`，或在 `onMounted` 中调用）

### 避免 Hydration 不一致

- 首屏避免 `Math.random()`、未处理的 `new Date()`（时区 / 格式需服务端与客户端一致）
- 条件渲染：用 `v-if="mounted"` 区分服务端占位与客户端真实 UI，勿同一节点 SSR/CSR 结构不同

### 新功能默认约定

- 首屏数据：路由 loader 或 `async setup`，不要默认只在 `onBeforeMount` 拉取
- 新依赖：先确认是否支持 Node；不支持则 dynamic import + `ClientOnly`
- API 调用：统一经 `createApiClient(请求上下文?)`，SSR 传入 request 头

---

## 必须预留 / 尽早实现

| 项 | 说明 |
|----|------|
| `createApp()` 工厂 | 每请求独立 pinia + router，防止请求串状态 |
| 状态注水 | Pinia（及后续页面级数据）序列化 + 客户端恢复 |
| 环境变量分层 | 浏览器 `VITE_*` vs Node `API_SERVER_URL` |
| SSR 运行进程 | 生产需 Node（或反代）；仅静态 build 无法实现 SSR |
| 按路由选择 SSR/CSR | 不必全站 SSR，降低复杂度 |
| 错误与 SEO | 失败兜底 HTML、`title`/`meta`、博客 XSS 消毒 |

---

## 建议实施顺序

1. `createApp()` + `entry-client` / `entry-server` + 最小 `server.ts`
2. 路由 Memory / Web 双模式 + `import.meta.env.SSR`
3. API 环境变量与 `Configuration` 工厂
4. 选一个页面（如 `HomeView`）迁移数据预取，验证「查看源代码」有内容
5. `ClientOnly` + mermaid / clipboard 等
6. head/meta、错误边界、生产 Node 部署

---

## 仓库工程化（与 SSR 并行建议）

- [ ] 修正 `.gitignore`：避免全局 `*.json` / `*.md` / `*.sln` 误伤协作文件
- [ ] 添加 `MyLife.sln`，便于 VS / Rider / CI 构建
- [ ] EF Migrations 与 `appsettings` 纳入明确的版本管理策略
- [ ] 排除 `API/Web/logs/` 等运行产物
