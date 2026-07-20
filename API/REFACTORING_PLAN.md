# AqLife API - 代码修复执行计划

## 🎯 第一阶段：高优先级修复（本次执行）

### 修复清单

#### 1. 命名规范修复
- [x] 文件名：`DataLayerSetup .cs` → `DataLayerSetup.cs`
- [x] 类名：`DowanloadFileHandler` → `DownloadFileHandler`
- [x] 类名：`TagQueryHandler` → `QueryTagsHandler`（需确认）

#### 2. Service 接口化改进
- [ ] IAccountService 接口创建
- [ ] IFileService 接口创建
- [ ] ITagService 接口创建
- [ ] ITodoService 接口创建
- [ ] 更新 DI 配置

#### 3. Repository 模式实现（可选，工作量大）
- [ ] IRepository<T> 通用接口
- [ ] GenericRepository<T> 实现
- [ ] IUnitOfWork 接口
- [ ] UnitOfWork 实现

---

## 执行顺序

1. ✅ 修复文件命名
2. ✅ 修复类名拼写
3. ✅ 创建 Service 接口
4. ✅ 更新依赖注入
5. ✅ 编译验证

---

**预计完成时间**：1-2 小时
