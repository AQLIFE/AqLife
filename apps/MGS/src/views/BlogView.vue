<script setup lang="ts">
import { ElButton, ElMessage } from 'element-plus'
import { Lock, Unlock, Upload } from '@element-plus/icons-vue'
import { onBeforeUnmount, onMounted, ref, watch } from 'vue'
import * as monaco from 'monaco-editor'
import { useRoute } from 'vue-router'
import { getArticleTitle } from '@aqlife/domain'
import { FileApi } from '@/api'
import { apiConfiguration } from '@/services/api'
import { MarkdownRender } from '@aqlife/ui-shared'
import MgsPageHeader from '@/components/ui/MgsPageHeader.vue'

const route = useRoute()
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
const loading = ref(false)
const baseurl = import.meta.env.VITE_API

function toggleReadOnly() {
  readOnly.value = !readOnly.value
  editor?.updateOptions({
    readOnly: readOnly.value,
  })
}

async function loadArticle(guid: string) {
  loading.value = true

  try {
    const fileApi = new FileApi(apiConfiguration)
    const response = await fileApi.apiFilePreviewGetRaw({ uID: guid })
    if (response.raw.status !== 200) {
      throw new Error(`无法加载博文（HTTP ${response.raw.status}）`)
    }

    const content = await response.text()
    markdown.value = content
    editor?.setValue(content)
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '博文加载失败')
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  if (container.value) {
    editor = monaco.editor.create(container.value, {
      value: markdown.value,
      language: 'markdown',
      theme: 'vs',
      fontFamily: 'Cascadia Code PL',
      automaticLayout: true,
      minimap: { enabled: false },
      wordWrap: 'on',
      fontSize: 14,
      readOnly: readOnly.value,
    })

    editor.onDidChangeModelContent(() => {
      markdown.value = editor!.getValue()
    })
  }

  const guid = typeof route.params.id === 'string' ? route.params.id : ''
  if (guid) {
    await loadArticle(guid)
  }
})

watch(
  () => route.params.id,
  async value => {
    const guid = typeof value === 'string' ? value : ''
    if (guid) {
      await loadArticle(guid)
    }
  },
)

onBeforeUnmount(() => {
  editor?.dispose()
})

async function handleUploadNewBlog() {
  loading.value = true

  try {
    const title = getArticleTitle(markdown.value)
    const fileApi = new FileApi(apiConfiguration)
    const blog = new File(
      [markdown.value],
      `${title || '未命名文章'}.md`,
      { type: 'text/markdown' },
    )
    const response = await fileApi.apiFileUploadPostRaw({ file: [blog] })
    if (response.raw.status === 200) {
      ElMessage.success('上传成功')
    } else {
      throw new Error(`上传失败（HTTP ${response.raw.status}）`)
    }
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '上传失败')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="markdown-workspace" v-loading="loading">
    <MgsPageHeader
      title="博文"
      description="查看与编辑 Markdown 内容"
      :back="true"
    >
      <template #actions>
        <ElButton
          :icon="readOnly ? Lock : Unlock"
          :type="readOnly ? 'warning' : 'info'"
          @click="toggleReadOnly"
        >
          {{ readOnly ? '解锁编辑' : '锁定编辑' }}
        </ElButton>
        <ElButton type="primary" :icon="Upload" @click="handleUploadNewBlog">
          上传新版本
        </ElButton>
      </template>
    </MgsPageHeader>

    <div class="editor-grid">
      <div ref="container" class="editor" />
      <div class="render">
        <MarkdownRender :markdown="markdown" :baseurl="baseurl" />
      </div>
    </div>
  </div>
</template>

<style scoped>
.markdown-workspace {
  min-height: 0;
  height: 100%;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.editor-grid {
  min-height: 0;
  flex: 1;
  display: grid;
  grid-template-columns: minmax(0, 1fr) minmax(0, 1fr);
  overflow: hidden;
  border-top: 1px solid var(--mgs-border);
}

.editor {
  min-width: 0;
  min-height: 0;
  overflow: hidden;
  border-right: 1px solid var(--mgs-border);
}

.render {
  min-width: 0;
  min-height: 0;
  overflow: auto;
  padding: 18px 24px 32px;
  background: var(--mgs-surface);
}

@media (max-width: 900px) {
  .editor-grid {
    grid-template-columns: 1fr;
  }

  .render {
    display: none;
  }
}
</style>
