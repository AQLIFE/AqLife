<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { ElButton, ElMessage } from 'element-plus'
import { Edit, Check, Lock } from '@element-plus/icons-vue'
import * as monaco from 'monaco-editor'
import { useRoute, useRouter } from 'vue-router'
import { FileApi, type FileDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useFileStore } from '@/stores/useFileStore'
import { FileContentUpdateWorkflow } from '@/workflow'
import { MarkdownRender } from '@aqlife/ui-shared'
import MgsPageHeader from '@/components/ui/MgsPageHeader.vue'

const route = useRoute()
const router = useRouter()
const fileApi = new FileApi(apiConfiguration)
const fileStore = useFileStore()
const baseurl = import.meta.env.VITE_API

const container = ref<HTMLElement>()
const markdown = ref('')
const originalMarkdown = ref('')
const file = ref<FileDto>()
const loading = ref(false)

let editor: monaco.editor.IStandaloneCodeEditor | undefined

const isEditing = computed(() => route.path.endsWith('/edit'))
const isDirty = computed(() => markdown.value !== originalMarkdown.value)
const pageTitle = computed(() => file.value?.fileName || '博文')
const pageDescription = computed(() =>
  isEditing.value
    ? '编辑 Markdown 内容，保存后生成文件的新版本'
    : '查看 Markdown 内容与发布状态',
)

async function loadArticle(guid: string) {
  loading.value = true

  try {
    const [previewResponse, metadata] = await Promise.all([
      fileApi.apiFilePreviewGetRaw({ uID: guid }),
      fileApi.apiFileGet({ uID: guid }),
    ])

    if (previewResponse.raw.status !== 200) {
      throw new Error(`无法加载博文（HTTP ${previewResponse.raw.status}）`)
    }

    const content = await previewResponse.raw.text()
    const dto = metadata.items?.[0]

    if (!dto) {
      throw new Error('无法加载博文元数据')
    }

    file.value = dto
    fileStore.upsertFile(dto)
    markdown.value = content
    originalMarkdown.value = content
    editor?.setValue(content)
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '博文加载失败')
  } finally {
    loading.value = false
  }
}

function enterEditMode() {
  const guid = typeof route.params.id === 'string' ? route.params.id : ''
  if (guid) router.push(`/blog/${guid}/edit`)
}

function leaveEditMode() {
  const guid = typeof route.params.id === 'string' ? route.params.id : ''
  if (guid) router.push(`/blog/${guid}`)
}

async function saveVersion() {
  if (!file.value?.uid || !isDirty.value) return

  loading.value = true

  try {
    const fileName = `${file.value.fileName || '未命名'}${file.value.fileType || '.md'}`
    const contentFile = new File(
      [markdown.value],
      fileName,
      { type: 'text/markdown' },
    )

    const workflow = new FileContentUpdateWorkflow(
      file.value,
      contentFile,
      fileApi,
      fileStore,
    )

    const updated = await workflow.run()
    file.value = updated
    originalMarkdown.value = markdown.value
    ElMessage.success('已保存为新版本')
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '保存失败')
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  editor = monaco.editor.create(container.value!, {
    value: markdown.value,
    language: 'markdown',
    theme: 'vs',
    fontFamily: 'Cascadia Code PL',
    automaticLayout: true,
    minimap: { enabled: false },
    wordWrap: 'on',
    fontSize: 14,
    readOnly: !isEditing.value,
  })

  editor.onDidChangeModelContent(() => {
    markdown.value = editor!.getValue()
  })

  const guid = typeof route.params.id === 'string' ? route.params.id : ''
  if (guid) {
    await loadArticle(guid)
  }
})

watch(
  () => route.params.id,
  async value => {
    const guid = typeof value === 'string' ? value : ''
    if (guid) await loadArticle(guid)
  },
)

watch(isEditing, value => {
  editor?.updateOptions({ readOnly: !value })
})

onBeforeUnmount(() => {
  editor?.dispose()
})
</script>

<template>
  <div class="markdown-workspace" v-loading="loading">
    <MgsPageHeader
      :title="pageTitle"
      :description="pageDescription"
      :back="true"
    >
      <template #actions>
        <template v-if="isEditing">
          <ElButton :icon="Lock" @click="leaveEditMode">
            退出编辑
          </ElButton>
          <ElButton
            type="primary"
            :icon="Check"
            :disabled="!isDirty"
            @click="saveVersion"
          >
            保存版本
          </ElButton>
        </template>
        <ElButton v-else type="primary" :icon="Edit" @click="enterEditMode">
          编辑
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
