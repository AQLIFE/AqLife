<script setup lang="ts">
import { ElPageHeader, ElMessage } from 'element-plus';
import { Lock, Unlock, Upload } from '@element-plus/icons-vue';
import { onBeforeUnmount, onMounted, ref, watch } from 'vue'
import * as monaco from 'monaco-editor'
import { useRouter } from 'vue-router';
import { getArticleTitle } from '@aqlife/domain';
import { FileApi } from '@/api';
import { apiConfiguration } from '@/services/api';
import { OperationalState, useActionStore } from '@/stores/useActionStore';
import {MarkdownRender} from '@aqlife/ui-shared'

const container = ref<HTMLElement>()

const markdown = ref(`# Hello World
这是一个 **Markdown 编辑器**。
## Features
- Monaco Editor
- 实时 Markdown 预览
- Vue 3
`)

// const errorMessage = '没有匹配的博文哦'

let editor: monaco.editor.IStandaloneCodeEditor | undefined
const readOnly = ref(true)

function toggleReadOnly() {
  readOnly.value = !readOnly.value
  editor?.updateOptions({
    readOnly: readOnly.value,
  })
}
const actionStore = useActionStore()
async function loadArticle(guid: string) {
  loading.value = true

  try {
    const fileApi = new FileApi(apiConfiguration)

    const response = await fileApi.apiFilePreviewGet({
      uID: guid
    })

    const content = await response.text()

    markdown.value = content
    editor?.setValue(content)
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  // 先创建 Monaco
  if (container.value) {
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
      markdown.value = editor!.getValue()
    })
  }

  // 再加载文章
  if (
    actionStore.OState === OperationalState.View &&
    actionStore.cacheViewGuid
  ) {
    await loadArticle(actionStore.cacheViewGuid)
  }
})
watch(
  () => actionStore.cacheViewGuid,
  async guid => {
    if (actionStore.OState === OperationalState.View && guid) {
      await loadArticle(guid)
    }
  }
)
onBeforeUnmount(() => {
  editor?.dispose()
})

const router = useRouter()
const loading = ref<boolean>(false)

async function handleUploadNewBlog() {
  loading.value = true
  const title = getArticleTitle(markdown.value)
  const fileApi = new FileApi(apiConfiguration)
  const blog = new File(
    [markdown.value],
    `${title || '未命名文章'}.md`,
    {
      type: 'text/markdown',
    }
  )
  const guid = await fileApi.apiFileUploadPostRaw({ file: [blog] })
  if (guid.raw.status == 200) ElMessage.success('上传成功')
  console.log(guid.raw.body)
  loading.value = false
}

</script>

<template>
  <div class="markdown-editor" v-loading="loading">
    <ElPageHeader content="博文视图" class="header" @back="router.back()">
      <template #extra>
        <ElButton :icon="readOnly ? Lock : Unlock" :type="readOnly ? 'warning' : 'info'" @click="toggleReadOnly"
          :title="readOnly ? '解锁编辑' : '锁定编辑'" />
        <ElButton :icon="Upload" type="success" @click="handleUploadNewBlog" />
      </template>
    </ElPageHeader>
    <!-- 左侧 Monaco -->
    <div ref="container" class="editor" />

    <!-- 右侧 Preview -->
    <!-- <div class="preview " v-html="marked(markdown)" /> -->
    <MarkdownRender :markdown="markdown"/>
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
