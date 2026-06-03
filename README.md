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

## 后端待办
- 持续优化业务代码
- 配合 WEB API 请求工具完善所有的 API 测试场景
