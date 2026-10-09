<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { ElButton, ElDatePicker, ElMessage, ElMessageBox, ElSegmented } from 'element-plus'
import { Edit, Check, Lock, Delete } from '@element-plus/icons-vue'
import * as monaco from 'monaco-editor'
import { useRoute, useRouter } from 'vue-router'
import { FileApi, type FileDto, type TagDto } from '@/api'
import { isMdType } from '@aqlife/domain'
import { apiConfiguration } from '@/services/api'
import { useFileStore } from '@/stores/useFileStore'
import { FileContentUpdateWorkflow, FileDeleteWorkflow, FileDraftWorkflow, FilePublishWorkflow, FileScheduledWorkflow, FileTagUpdateWorkflow } from '@/workflow'
import { MarkdownRender } from '@aqlife/ui-shared'
import MgsPageHeader from '@/components/ui/MgsPageHeader.vue'
import { publishStatusOptions, publishStatus } from '@/types/TableFilterOption.ts'
import TagSelect from '@/components/TagSelect.vue'

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
const publishStatusModel = ref<publishStatus>()
const publishing = ref(false)
const scheduling = ref(false)
const deleting = ref(false)
const selectedTags = ref<TagDto[]>([])
const scheduledAt = ref<Date | null>(null)

const segmentedOptions = computed(() =>
  publishStatusOptions.map(item => ({
    ...item,
    disabled:
      item.value === publishStatus.Scheduled &&
      file.value?.publishStatus === publishStatus.Published,
  })),
)

const isMarkdownFile = computed(() => isMdType(file.value?.fileType ?? ''))

const pageDescription = computed(() => {
  if (file.value && !isMarkdownFile.value) return '该文件类型不支持 Markdown 编辑或预览'
  return isEditing.value
    ? '编辑 Markdown 内容，保存后生成文件的新版本'
    : '查看 Markdown 内容与发布状态'
})

function parsePublishAt(value: FileDto['publishAt']): Date | null {
  if (!value) return null
  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? null : date
}

async function loadArticle(guid: string) {
  loading.value = true

  try {
    const metadata = await fileApi.apiFileGet({ uID: guid })
    const dto = metadata.items?.[0]

    if (!dto) {
      throw new Error('无法加载博文元数据')
    }

    file.value = dto
    fileStore.upsertFile(dto)
    publishStatusModel.value = dto.publishStatus
    selectedTags.value = dto.tags ?? []
    scheduledAt.value = parsePublishAt(dto.publishAt)

    // 非 Markdown 文件不应请求文本预览接口，也不能交给 Markdown 编辑器/渲染器。
    if (!isMdType(dto.fileType ?? '')) {
      markdown.value = ''
      originalMarkdown.value = ''
      editor?.setValue('')
      return
    }

    const previewResponse = await fileApi.apiFilePreviewGetRaw({ uID: guid })
    if (previewResponse.raw.status !== 200) {
      throw new Error(`无法加载博文（HTTP ${previewResponse.raw.status}）`)
    }

    const content = await previewResponse.raw.text()
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

async function onTagsChanged(value: FileDto['tags']) {
  if (!file.value?.uid || !value) return

  try {
    const dto = await new FileTagUpdateWorkflow(file.value, value, fileApi, fileStore).run()
    file.value = dto
    selectedTags.value = dto.tags ?? []
    ElMessage.success('标签已更新')
  } catch (error) {
    selectedTags.value = file.value.tags ?? []
    ElMessage.error(error instanceof Error ? error.message : '标签更新失败')
  }
}

async function onPublishStatusChanged(value: publishStatus) {
  const current = file.value
  if (!current?.uid || current.publishStatus === value) return

  try {
    publishing.value = true

    let dto: FileDto

    switch (value) {
      case publishStatus.Draft:
        dto = await new FileDraftWorkflow(current, fileApi, fileStore).run()
        break
      case publishStatus.Published:
        dto = await new FilePublishWorkflow(current, fileApi, fileStore).run()
        break
      case publishStatus.Scheduled:
        if (current.publishStatus === publishStatus.Published) {
          throw new Error('禁止从已发布状态修改为预定发布状态')
        }
        if (!current.publishAt) {
          publishStatusModel.value = current.publishStatus
          ElMessage.warning('请先设置预定发布时间')
          return
        }
        dto = await new FileScheduledWorkflow(current, fileApi, fileStore).run()
        break
      default:
        return
    }

    file.value = dto
    publishStatusModel.value = dto.publishStatus
    scheduledAt.value = parsePublishAt(dto.publishAt)
    ElMessage.success('发布状态已更新')
  } catch (error) {
    publishStatusModel.value = current.publishStatus
    ElMessage.error(error instanceof Error ? error.message : '发布状态更新失败')
  } finally {
    publishing.value = false
  }
}

async function onScheduledAtChanged(value: Date | null) {
  if (!file.value || !value) return

  const current = file.value
  const previousPublishAt = current.publishAt
  const nextPublishAt = value.toISOString()

  // 草稿阶段只记录预约时间，切换到“预约”时再提交。
  current.publishAt = nextPublishAt
  scheduledAt.value = value

  if (current.publishStatus === publishStatus.Draft) {
    ElMessage.success('预定发布时间已设置')
    return
  }

  try {
    scheduling.value = true
    const dto = await new FileScheduledWorkflow(current, fileApi, fileStore).run()
    file.value = dto
    publishStatusModel.value = dto.publishStatus
    scheduledAt.value = parsePublishAt(dto.publishAt)
    ElMessage.success('预定发布时间已更新')
  } catch (error) {
    current.publishAt = previousPublishAt
    scheduledAt.value = parsePublishAt(previousPublishAt)
    ElMessage.error(error instanceof Error ? error.message : '设置预定发布时间失败')
  } finally {
    scheduling.value = false
  }
}

async function deleteArticle() {
  const current = file.value
  if (!current?.uid || deleting.value) return

  try {
    await ElMessageBox.confirm(
      `确定删除「${current.fileName || '未命名文章'}」吗？删除后将无法在 MGS 中继续编辑这篇文章。`,
      '删除文章',
      {
        confirmButtonText: '删除',
        cancelButtonText: '取消',
        type: 'warning',
      },
    )

    deleting.value = true
    await new FileDeleteWorkflow(current, fileApi, fileStore).run()
    ElMessage.success('文章已删除')
    await router.push('/blog/list')
  } catch (error) {
    if (error === 'cancel' || error === 'close') return
    ElMessage.error(error instanceof Error ? error.message : '删除文章失败')
  } finally {
    deleting.value = false
  }
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
        <div v-if="file" class="publish-control">
          <TagSelect
            class="blog-tag-select"
            v-model:tag-list="selectedTags"
            :select-disabled="publishing || scheduling"
            @update:tag-list="onTagsChanged"
          />
          <ElSegmented
            :model-value="publishStatusModel"
            :options="segmentedOptions"
            :disabled="publishing || scheduling"
            @change="onPublishStatusChanged"
          />
          <ElDatePicker
            v-model="scheduledAt"
            type="datetime"
            :disabled="publishing || scheduling || file.publishStatus === publishStatus.Published"
            placeholder="预定发布时间"
            @change="onScheduledAtChanged"
          />
        </div>
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
        <template v-else>
          <ElButton :icon="Delete" :disabled="deleting || publishing || scheduling" @click="deleteArticle">
            删除
          </ElButton>
          <ElButton v-if="isMarkdownFile" type="primary" :icon="Edit" @click="enterEditMode">
            编辑
          </ElButton>
        </template>
      </template>
    </MgsPageHeader>

    <div v-show="isMarkdownFile" class="editor-grid">
      <div ref="container" class="editor" />
      <div class="render">
        <MarkdownRender v-if="isMarkdownFile" :markdown="markdown" :baseurl="baseurl" />
      </div>
    </div>
    <div v-else-if="file" class="unsupported-file">
      <p>此文件不是 Markdown 文件，已停止 Markdown 加载与渲染。</p>
      <p>文件类型：{{ file.fileType || '未知' }}</p>
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

.unsupported-file {
  margin: 24px;
  padding: 24px;
  border: 1px solid var(--mgs-border);
  border-radius: 8px;
  color: var(--mgs-muted);
}

.unsupported-file p {
  margin: 0 0 8px;
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

.publish-control {
  display: flex;
  align-items: center;
  gap: 8px;
  min-width: 0;
}

.publish-control :deep(.blog-tag-select) {
  width: 220px;
  min-width: 180px;
  flex: 0 1 220px;
}

.publish-control :deep(.blog-tag-select .el-select__wrapper) {
  width: 100%;
  min-width: 0;
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
