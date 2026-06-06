# 项目结构

```mermaid
graph TD
  API[/API]
  API --> API_Backend[backend/]
  API --> API_Frontend[frontend/]
  API_Backend --> Backend_Controllers[controllers/]
  Backend_Controllers --> Backend_Controllers_Auth[AuthController.cs]
  API_Backend --> Backend_Services[services/]
  API_Backend --> Backend_Models[models/]
  API_Frontend --> Frontend_Src[src/]
  Frontend_Src --> Frontend_Components[components/]
  Frontend_Components --> OptionAccount[OptionAccount.vue]
  Frontend_Src --> Frontend_Pages[pages/]
  Frontend_Pages --> Blog_Detail[BlogDetail.vue]
  Frontend_Src --> Frontend_Utils[utils/]
```

## 前端待办

- 完善 OptionAccount
- 添加 Blog 详情页（即 Markdown 文档渲染器组件）
- 设计时间线组件

📋 组件实现待办清单 (To-Do List)
要实现这个强悍的渲染器，建议将工程拆分为以下 4 个解耦的模块：

第一部分：解析引擎初始化 (The Parser)

- [ ]实例化 markdown-it，开启 html: true 允许解析 HTML。

- [ ]接入 markdown-it-anchor，配置生成规则（例如去除空格、特殊字符转拼音），确保每个标题生成的 id 在整个 DOM 中是唯一的。

- [ ]编写一个 Token 拦截器：遍历 markdown-it 解析出的 Tokens 数组，找出类型为 fence (代码块) 且 info 为 mermaid 的节点，打上特殊标记。

第二部分：目录大纲提取器 (The TOC Extractor)

- [ ]编写一个独立函数，在组件 setup 阶段接收原始 md 文本。

- [ ]遍历 AST (Tokens)，抽取出所有 h1 到 h6 的节点。

- [ ]获取这些节点的层级、文本内容以及对应的 id（由 anchor 插件生成）。

- [ ]将抽取的数据组装成一个树状结构的响应式数组，供给右侧的“目录树”组件进行遍历渲染。

第三部分：Vue 渲染管线 (The Rendering Pipeline)

- [ ]利用 Vue 的 computed 属性，在拿到 md 文本后立即解析出结构化数组（文本块、代码块、Mermaid 块），这一步在组件 mounted 前就会完成。

- [ ]在 Vue <template> 中使用 v-for 遍历这个结构化数组。

- [ ]根据块的类型，使用 v-if/v-else 分发渲染：普通 HTML 给一个包裹层 v-html；代码块喂给你的 <CodeBlock> 组件；Mermaid 喂给你的 <MermaidViewer> 组件。

第四部分：特殊组件内部封装 (The Custom Blocks)

- [ ]高亮组件：在 <CodeBlock> 内部接入 shiki 或 highlight.js，接收纯文本代码并输出带有颜色样式的 HTML。

- [ ]Mermaid 组件：封装一个专门处理图表的组件。接收图表语法，内部调用 mermaid.render() 生成 SVG 字符串并渲染。

## 后端待办

- 持续优化业务代码
- 配合 WEB API 请求工具完善所有的 API 测试场景
