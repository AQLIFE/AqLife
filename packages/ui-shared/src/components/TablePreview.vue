<template>
  <div class="table-block-wrapper">
    <!-- 渲染完整的、未被 div 污染的 table 结构 [cite: 5, 216] -->
    <div class="markdown-body" v-html="tableHtml"></div>
  </div>
</template>
<script setup lang="ts">
import { computed } from 'vue'
import MarkdownIt from 'markdown-it'
import type { Token } from 'markdown-it/index.js'

const props = defineProps<{
  tableTokens: Token[] // 接收从 table_open 到 table_close 的所有 token
}>()

const md = new MarkdownIt()

// 直接渲染这一组完整的表格 token，保证 <table> 内部结构的纯净 [cite: 215]
const tableHtml = computed(() => {
  return md.renderer.render(props.tableTokens, md.options, {})
})
</script>
<style scoped>
/* 可以在这里针对表格进行组件级的样式定制 */
.table-block-wrapper {
  margin: 20px 0;
  overflow-x: auto;
}
.markdown-body :deep(table) {
  border-collapse: collapse;
  width: 100%;
  margin-bottom: 1rem;
}

/* 1. 设置表头 (thead/第一行) 为白色背景 */
.markdown-body :deep(table thead tr),
.markdown-body :deep(table thead th) {
  background-color: #ffffff !important; /* 强制覆盖 GitHub 默认的灰色背景 */
  color: var(--el-text-color-primary);
}

/* 2. 设置第一列 (td:first-child) 为白色背景 */
.markdown-body :deep(table tbody tr td:first-child),
.markdown-body :deep(table thead tr th:first-child) {
  background-color: #ffffff !important;
  font-weight: 600; /* 第一列通常作为属性名，加粗处理更具“工业感” */
}

/* 3. 补充：基础边框与间距（防止纯白背景下看不清格子） */
.markdown-body :deep(table th),
.markdown-body :deep(table td) {
  border: 1px solid var(--el-border-color-lighter);
  padding: 12px 15px;
  text-align: left;
}
</style>
