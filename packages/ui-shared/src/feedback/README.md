# 前端反馈与加载状态约定

## 通知

- 页面和操作结果统一使用 `notify.success/info/warning/error`，底层为 Element Plus `ElNotification`。
- 错误对象优先使用 `notify.fromError(error, fallback)`，避免每个页面重复解析异常。
- 不再使用 `ElMessage`。需要用户确认、输入或选择的阻断式交互仍使用 `ElMessageBox`，它不是轻提示通知。
- 通知只用于结果或重要提示，不用来代替页面内的校验说明、上传进度或加载状态。

## 加载状态

- 页面首次加载、页面级错误和空数据使用 MGS `MgsPageState`。
- 表格/列表请求使用区域级 `v-loading`，并通过 `useLoadingTask('唯一作用域')` 提供状态。
- `useLoadingTask` 的 `run()` 适用于包裹异步任务；需要跨多个 `try/finally` 阶段时使用 `begin()`，并且每次 begin 都必须在 finally 中调用返回的 finish 函数。
- 同一作用域的并发任务共享计数；只有全部任务结束，`isLoading` 才会变为 false。不同页面或区域必须使用不同作用域名称。
- 按钮级操作使用按钮自身的 `loading`，不应为一次局部保存/删除而遮挡整页。
- 上传使用上传组件自身的 `uploading`、进度、暂停/重试等状态。存在可见进度时不要再叠加整页 `v-loading`。
- Markdown 渲染、编辑器内部计算等非请求型状态可以继续使用组件自身的局部 loading，不必纳入请求计数器。

## 典型用法

```ts
const { isLoading, run } = useLoadingTask('mgs:table:files')

async function refresh() {
  await run(() => fileStore.fetchAllFiles(fileApi))
}
```

```vue
<ElTable v-loading="isLoading" :data="rows" />
```

```ts
const { isLoading, begin } = useLoadingTask('mgs:blog:editor')

async function loadArticle() {
  const finish = begin()
  try {
    await fetchArticle()
  } finally {
    finish()
  }
}
```
