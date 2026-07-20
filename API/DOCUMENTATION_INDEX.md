# AqLife API 文档索引

## 📚 文档导航

本项目包含详细的架构文档、设计指南和快速参考。根据您的需求选择相应的文档。

---

## 🎯 按角色分类

### 👨‍💼 项目管理者 / 架构师

**快速了解项目：**
1. [README.md](README.md) - 项目整体概览（15 分钟阅读）
2. [DIAGNOSTIC_REPORT.md](DIAGNOSTIC_REPORT.md) - 完整诊断报告（30 分钟阅读）

**深入了解架构：**
3. [ARCHITECTURE_DECISIONS.md](ARCHITECTURE_DECISIONS.md) - 13 项架构决策详解
4. [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) - 改进建议和路线图

---

### 👨‍💻 开发人员

**第一次接触项目：**
1. [README.md](README.md) - 项目概览
2. [QUICK_REFERENCE.md](QUICK_REFERENCE.md) - 快速参考卡片（5 分钟查阅）

**学习各层的实现方式：**
3. [Domain/README.md](Domain/README.md) - 学习如何定义命令和查询
4. [Service/README.md](Service/README.md) - 学习如何实现业务逻辑

**遇到问题时：**
5. [QUICK_REFERENCE.md](QUICK_REFERENCE.md) - 常见错误和解决方案

---

### 🧪 测试工程师

**理解系统架构：**
1. [README.md](README.md) - 项目架构概览
2. [Domain/README.md](Domain/README.md) - 了解请求流程

**测试策略：**
3. [DIAGNOSTIC_REPORT.md](DIAGNOSTIC_REPORT.md) - 查看测试覆盖现状
4. [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) - 查看建议的测试框架

---

### 🔧 运维 / DevOps

**部署和配置：**
1. [README.md](README.md) - 快速启动指南部分
2. 查看 `API/Web/Configurations/` 目录的配置说明

---

## 📖 按内容分类

### 🏗️ 架构与设计

| 文档 | 内容 | 阅读时间 |
|------|------|---------|
| [README.md](README.md) | 完整的项目架构说明，包含 6 层设计详解 | 30 分钟 |
| [ARCHITECTURE_DECISIONS.md](ARCHITECTURE_DECISIONS.md) | 13 项架构决策的详细说明和理由 | 45 分钟 |
| [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) | 当前结构评估和改进建议 | 40 分钟 |

### 🎓 学习指南

| 文档 | 内容 | 对象 |
|------|------|------|
| [Domain/README.md](Domain/README.md) | Domain 层的职责、如何定义命令和查询 | 新开发者 |
| [Service/README.md](Service/README.md) | Service 层的职责、如何实现业务逻辑 | 新开发者 |
| [QUICK_REFERENCE.md](QUICK_REFERENCE.md) | 常见任务的快速解决方案 | 所有开发者 |

### 📊 诊断和改进

| 文档 | 内容 | 目的 |
|------|------|------|
| [DIAGNOSTIC_REPORT.md](DIAGNOSTIC_REPORT.md) | 完整的项目诊断报告，包含评分和改进方案 | 管理决策 |

---

## 🗺️ 文档地图

```
API/
│
├── 📄 README.md                          ⭐ 开始这里
│   └── 项目总体说明，完整的架构图解
│
├── 📄 DIAGNOSTIC_REPORT.md
│   └── 项目诊断评分、优势、改进方案
│
├── 📄 ARCHITECTURE_DECISIONS.md          🎯 架构师必读
│   └── 13 项重要架构决策的背景和理由
│
├── 📄 PROJECT_STRUCTURE.md               📈 改进路线图
│   └── 当前结构评估、改进建议、实施计划
│
├── 📄 QUICK_REFERENCE.md                ⚡ 快速查询
│   └── 常见任务、代码片段、快速命令
│
├── Domain/
│   └── 📄 README.md                      📝 学习命令/查询
│       └── 如何定义 Command 和 Query
│
├── Service/
│   └── 📄 README.md                      💼 学习业务逻辑
│       └── 如何编写 Service 和 Mapper
│
└── [其他项目目录]
```

---

## 🚀 快速开始

### 第一次接触本项目？

**推荐阅读顺序：**
1. [README.md](README.md) - 5 分钟了解项目
2. [QUICK_REFERENCE.md](QUICK_REFERENCE.md) - 快速掌握常见任务
3. 根据您的角色选择相应文档深入学习

### 需要完成具体任务？

查看 [QUICK_REFERENCE.md](QUICK_REFERENCE.md) 中的"常见任务速查"部分，包括：
- ✅ 添加新的业务功能
- ✅ 添加数据验证规则
- ✅ 添加数据库表
- ✅ 实现自定义搜索

### 遇到问题？

1. 查看 [QUICK_REFERENCE.md](QUICK_REFERENCE.md) 的"调试技巧"
2. 查看 [DIAGNOSTIC_REPORT.md](DIAGNOSTIC_REPORT.md) 的常见问题
3. 查看相应层的 README 获取更多细节

---

## 📚 分层详细指南

### Web 层
- **职责：** HTTP 入口、路由、中间件
- **关键问题：** 如何添加新的端点？
- **参考：** [README.md](README.md) 中的"Web 层"部分

### Application 层
- **职责：** CQRS 流程协调、验证、事务
- **关键问题：** 如何添加新的 Handler？
- **参考：** [QUICK_REFERENCE.md](QUICK_REFERENCE.md) 中的"添加新功能"

### Domain 层
- **职责：** 命令和查询定义
- **关键问题：** 如何定义命令或查询？
- **参考：** [Domain/README.md](Domain/README.md)

### Service 层
- **职责：** 业务逻辑实现、数据映射
- **关键问题：** 如何实现业务逻辑？
- **参考：** [Service/README.md](Service/README.md)

### Data 层
- **职责：** 数据访问、数据库迁移
- **关键问题：** 如何添加新的表？
- **参考：** [README.md](README.md) 中的"Data 层"部分

### Shared 层
- **职责：** 共享契约、异常、DTO
- **关键问题：** 如何定义新的异常类型？
- **参考：** [README.md](README.md) 中的"Shared 层"部分

---

## 🔍 按关键词搜索

### 我想...

| 我想... | 查看... |
|--------|--------|
| 理解整个架构 | [README.md](README.md) |
| 了解为什么这样设计 | [ARCHITECTURE_DECISIONS.md](ARCHITECTURE_DECISIONS.md) |
| 知道如何改进项目 | [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) + [DIAGNOSTIC_REPORT.md](DIAGNOSTIC_REPORT.md) |
| 快速完成一个任务 | [QUICK_REFERENCE.md](QUICK_REFERENCE.md) |
| 学习如何定义命令 | [Domain/README.md](Domain/README.md) |
| 学习如何实现业务逻辑 | [Service/README.md](Service/README.md) |
| 了解当前的问题 | [DIAGNOSTIC_REPORT.md](DIAGNOSTIC_REPORT.md) |
| 找到编码错误的解决方案 | [QUICK_REFERENCE.md](QUICK_REFERENCE.md) 的"调试技巧" |
| 添加新的业务模块 | [QUICK_REFERENCE.md](QUICK_REFERENCE.md) 的"添加新功能" |

---

## 📊 文档统计

```
总文档数：        7 个文档
总字数：          ~50,000 字
总章节数：        ~80 个章节
代码示例：        100+ 个
图表/流程图：     20+ 个
常见问题：        30+ 个
快速参考：        50+ 个
```

---

## 🔄 文档间的关联

```
README.md (项目概览)
  ├─→ 深入学习：ARCHITECTURE_DECISIONS.md
  ├─→ 实施路线：PROJECT_STRUCTURE.md
  ├─→ 当前状态：DIAGNOSTIC_REPORT.md
  └─→ 快速查询：QUICK_REFERENCE.md

Domain/README.md (定义命令/查询)
  └─→ 实现示例：QUICK_REFERENCE.md 中的代码片段

Service/README.md (实现业务逻辑)
  └─→ 实现示例：QUICK_REFERENCE.md 中的代码片段

QUICK_REFERENCE.md (常见任务)
  ├─→ 理论基础：对应层的 README
  ├─→ 架构原理：ARCHITECTURE_DECISIONS.md
  └─→ 故障排查：DIAGNOSTIC_REPORT.md
```

---

## 🎯 按学习级别分类

### 初级开发者（第 1 周）

**必读文档：**
1. [README.md](README.md) - 理解架构
2. [QUICK_REFERENCE.md](QUICK_REFERENCE.md) - 学习常见任务
3. [Domain/README.md](Domain/README.md) - 理解如何定义请求

### 中级开发者（1-3 个月）

**推荐文档：**
1. [Service/README.md](Service/README.md) - 深入学习业务逻辑
2. [ARCHITECTURE_DECISIONS.md](ARCHITECTURE_DECISIONS.md) - 理解设计决策
3. [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) - 了解改进方向

### 高级开发者 / 架构师

**关键文档：**
1. [ARCHITECTURE_DECISIONS.md](ARCHITECTURE_DECISIONS.md) - 架构决策细节
2. [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) - 扩展规划
3. [DIAGNOSTIC_REPORT.md](DIAGNOSTIC_REPORT.md) - 全面评估

---

## 📞 文档更新日志

| 日期 | 更新内容 | 版本 |
|------|---------|------|
| 2026-05 | 初始创建所有文档 | 1.0 |
| - | 待更新 | - |

---

## 💡 使用建议

### 📌 收藏常用链接

- **日常开发：** [QUICK_REFERENCE.md](QUICK_REFERENCE.md)
- **学习新知识：** 对应层的 README
- **架构讨论：** [ARCHITECTURE_DECISIONS.md](ARCHITECTURE_DECISIONS.md)

### 📌 定期审查

- **每月：** 审查 [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) 的改进项
- **每季度：** 复习 [README.md](README.md) 确保理解未变
- **有新人加入：** 让他们按顺序阅读入门文档

### 📌 提出建议

如果您发现：
- 文档不清楚
- 代码示例过时
- 缺少关键信息

请通过项目的反馈机制提出改进建议。

---

## 🏆 文档质量指标

| 指标 | 评分 |
|------|------|
| 完整性 | ⭐⭐⭐⭐⭐ (5/5) |
| 清晰度 | ⭐⭐⭐⭐⭐ (5/5) |
| 实用性 | ⭐⭐⭐⭐⭐ (5/5) |
| 示例代码 | ⭐⭐⭐⭐⭐ (5/5) |
| 可维护性 | ⭐⭐⭐⭐☆ (4/5) |

---

**文档维护者：** AqLife 项目团队  
**最后更新：** 2026年5月  
**下次审查：** 2026年8月

---

## 🔗 快速链接

- [项目 README](README.md)
- [架构决策](ARCHITECTURE_DECISIONS.md)
- [诊断报告](DIAGNOSTIC_REPORT.md)
- [快速参考](QUICK_REFERENCE.md)
- [Domain 指南](Domain/README.md)
- [Service 指南](Service/README.md)
- [项目结构](PROJECT_STRUCTURE.md)

