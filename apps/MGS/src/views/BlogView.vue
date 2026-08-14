<script setup lang="ts">
import { ElCol, ElRow, ElPageHeader, ElMessage } from 'element-plus';
import { ArrowLeft, Lock, Unlock, Upload } from '@element-plus/icons-vue';
import { onBeforeUnmount, onMounted, ref } from 'vue'
import * as monaco from 'monaco-editor'
import { marked } from 'marked'
import { useRouter } from 'vue-router';
import { getArticleTitle } from '@aqlife/domain';
import { FileApi } from '@/api';
import { apiConfiguration } from '@/services/api';
// import {mdRenderOption}

const container = ref<HTMLElement>()

const markdown = ref(`# Hello World
这是一个 **Markdown 编辑器**。
## Features
- Monaco Editor
- 实时 Markdown 预览
- Vue 3
`)

let editor: monaco.editor.IStandaloneCodeEditor | undefined
const readOnly = ref(true)

function toggleReadOnly() {
  readOnly.value = !readOnly.value
  editor?.updateOptions({
    readOnly: readOnly.value,
  })
}
onMounted(() => {
  if (!container.value) return

  editor = monaco.editor.create(container.value, {
    value: markdown.value,
    language: 'markdown',

    theme: 'vs',

    automaticLayout: true,

    minimap: {
      enabled: false,
    },

    wordWrap: 'on',

    fontSize: 14,
    readOnly: readOnly.value,
  })

  editor.onDidChangeModelContent(() => {
    markdown.value = editor?.getValue() ?? ''
  })
})

onBeforeUnmount(() => {
  editor?.dispose()
})

const router = useRouter()

async function handleUploadNewBlog() {
  lodding.value = true
  const title = getArticleTitle(markdown.value)
  const fileApi = new FileApi(apiConfiguration)
  const blog = new File(
    [markdown.value],
    `${title || '未命名文章'}.md`,
    {
      type: 'text/markdown',
    }
  )
  const guid = await fileApi.apiFileUploadPostRaw({file:[blog]})
  if(guid.raw.status == 200)ElMessage.success('上传成功')
  console.log(guid.raw.body)
lodding.value = false
}

const lodding = ref(false)
</script>

<template>
  <div class="markdown-editor" v-lodding="lodding">
    <ElPageHeader content="博文视图" class="header" @back="router.back()">
      <template #extra>
        <ElButton :icon="readOnly ? Lock : Unlock" :type="readOnly ? 'warning' : 'info'" @click="toggleReadOnly"
          :title="readOnly ? '解锁编辑' : '锁定编辑'" />
        <ElButton :icon="Upload" type="success" @click="handleUploadNewBlog"/>
      </template>
    </ElPageHeader>
    <!-- 左侧 Monaco -->
    <div ref="container" class="editor" />

    <!-- 右侧 Preview -->
    <div class="preview markdown-body" v-html="marked(markdown)" />
  </div>
</template>

<style lang="css" scoped>
.header {
  grid-column: 1 / 3;
  height: 50px;
  line-height: 50px;
}

.markdown-editor {
  display: grid;

  /* 两边各占 50% */
  grid-template-rows: auto 1fr;
  grid-template-columns: 1fr 1fr;
  text-align: left;
  height: 100vh;
}

.editor {
  min-width: 0;
  height: 100%;
}

.preview {
  min-width: 0;
  height: 100%;

  overflow: auto;

  padding: 24px;
  box-sizing: border-box;

  border-left: 1px solid #ddd;
}

/* Markdown 基础样式 */

.markdown-body :deep(h1) {
  font-size: 2em;
}

.markdown-body :deep(h2) {
  font-size: 1.5em;
}

.markdown-body :deep(code) {
  padding: 2px 4px;
  background: #f5f5f5;
}

.markdown-body :deep(pre) {
  padding: 16px;
  overflow-x: auto;
  background: #f5f5f5;
}
</style>
