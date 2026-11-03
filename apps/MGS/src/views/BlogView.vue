<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { ElButton, ElDatePicker, ElMessage, ElMessageBox, ElSegmented } from 'element-plus'
import { Edit, Check, Lock, Delete } from '@element-plus/icons-vue'
import * as monaco from 'monaco-editor'
import { useRoute, useRouter } from 'vue-router'
import { FileApi, FileScope, type FileDto, type TagDto } from '@/api'
import { tokenStore } from '@aqlife/api-client'
import { apiConfiguration } from '@/services/api'
import { useFileStore } from '@/stores/useFileStore'
import { FileContentUpdateWorkflow, FileDeleteWorkflow, FileDraftWorkflow, FilePublishWorkflow, FileScheduledWorkflow, FileTagUpdateWorkflow } from '@/workflow'
import { MarkdownRender } from '@aqlife/ui-shared'
import MgsPageHeader from '@/components/ui/MgsPageHeader.vue'
import { FileStatus } from "@/api"
import TagSelect from '@/components/TagSelect.vue'
import { useMarkdownClipboardImagePaste } from '@/composables/useMarkdownClipboardImagePaste'
import { useMarkdownVideoUploadQueue } from '@/composables/useMarkdownVideoUploadQueue'
import { publishStatusOptions } from '@/types/TableFilterOption'


const route = useRoute()
const router = useRouter()
const props = defineProps<{ id?: string }>()
const fileApi = new FileApi(apiConfiguration)
const fileStore = useFileStore()
const videoInput = ref<HTMLInputElement>()
const { items: videoUploads, busy: videoUploading, addFiles: queueVideos, retry: retryVideoUpload, waitForAll: waitForVideoUploads } = useMarkdownVideoUploadQueue(() => editor, fileApi, apiConfiguration.basePath)
const draftStorageKey = (uid: string) => `aqlife:blog-draft:${uid}`
const DRAFT_TTL_MS = 7 * 24 * 60 * 60 * 1000
const baseurl = import.meta.env.VITE_API
const markdownFileResolver = {
  async getFile(uid: string): Promise<FileDto | undefined> {
    const image = await fileApi.apiFileGet({ uID: uid, scope: FileScope.Image })
    if (image.items?.[0]) return image.items[0]
    const video = await fileApi.apiFileGet({ uID: uid, scope: FileScope.Video })
    return video.items?.[0]
  },
  async getVideoUrl(uid: string): Promise<string | undefined> {
    const basePath = apiConfiguration.basePath.replace(/\/+$/, '')
    const headers = new Headers()
    const token = tokenStore.get()
    if (token) headers.set('Authorization', token.startsWith('Bearer ') ? token : `Bearer ${token}`)
    const response = await fetch(`${basePath}/api/File/video-preview-ticket?uid=${encodeURIComponent(uid)}`, {
      method: 'POST',
      headers,
      credentials: 'include',
    })
    if (!response.ok) return undefined
    const result = await response.json() as { ticket?: string }
    if (!result.ticket) return undefined
    const playbackUrl = new URL(`${basePath}/api/File/video-stream?UID=${encodeURIComponent(uid)}&ticket=${encodeURIComponent(result.ticket)}`, window.location.href)
    return playbackUrl.toString()
  },
}

const container = ref<HTMLElement>()
const markdown = ref('')
const markdownRenderRef = ref<{ loading: boolean } | null>(null)
const originalMarkdown = ref('')
const file = ref<FileDto>()
const loading = ref(false)

let editor: monaco.editor.IStandaloneCodeEditor | undefined
let articleLoadVersion = 0

const isEditing = computed(() => route.path.endsWith('/edit'))
const { uploading: imageUploading, onPaste: onEditorPaste } = useMarkdownClipboardImagePaste({
  getEditor: () => editor,
  isEnabled: () => isEditing.value,
  uploadFiles: async files => {
    const uids = await fileApi.apiFileUploadPost({ file: files })
    await Promise.all(uids.map(uid =>
      fileApi.apiFileSchedulePatch({ scheduledFileCommand: { uid } }),
    ))
    return uids
  },
})
const isDirty = computed(() => markdown.value !== originalMarkdown.value)
const pageTitle = computed(() => file.value?.fileName || '博文')
const publishStatusModel = ref<FileStatus>()
const publishing = ref(false)
const scheduling = ref(false)
const deleting = ref(false)
const selectedTags = ref<TagDto[]>([])
const scheduledAt = ref<Date | null>(null)

const segmentedOptions = computed(() =>
  publishStatusOptions.map(item => ({
    ...item,
    disabled:
      item.value === FileStatus.Scheduled &&
      file.value?.publishStatus === FileStatus.Published,
  })),
)

const pageDescription = computed(() =>
  isEditing.value
    ? '编辑 Markdown 内容，保存后生成文件的新版本'
    : '查看 Markdown 内容与发布状态',
)

function parsePublishAt(value: FileDto['publishAt']): Date | null {
  if (!value) return null
  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? null : date
}

async function loadArticle(guid: string) {
  const version = ++articleLoadVersion
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
    if (version !== articleLoadVersion) return

    const dto = metadata.items?.[0]

    if (!dto) {
      throw new Error('无法加载博文元数据')
    }

    file.value = dto
    fileStore.upsertFile(dto)
    markdown.value = content
    originalMarkdown.value = content
    if (isEditing.value) await restoreLocalDraft(guid, content)
    editor?.setValue(markdown.value)
    publishStatusModel.value = dto.publishStatus
    selectedTags.value = dto.tags ?? []
    scheduledAt.value = parsePublishAt(dto.publishAt)
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '博文加载失败')
  } finally {
    if (version === articleLoadVersion) loading.value = false
  }
}

async function restoreLocalDraft(uid: string, serverContent: string) {
  try {
    const raw = localStorage.getItem(draftStorageKey(uid))
    if (!raw) return
    const saved = JSON.parse(raw) as { content?: string; savedAt?: number }
    if (!saved.content || !saved.savedAt || Date.now() - saved.savedAt > DRAFT_TTL_MS) {
      localStorage.removeItem(draftStorageKey(uid))
      return
    }
    if (saved.content === serverContent) {
      localStorage.removeItem(draftStorageKey(uid))
      return
    }
    try {
      await ElMessageBox.confirm('发现 7 天内保存的本地未提交草稿，是否恢复？', '恢复博文草稿', { confirmButtonText: '恢复草稿', cancelButtonText: '使用服务器版本', type: 'info' })
      markdown.value = saved.content
      editor?.setValue(saved.content)
      ElMessage.success('已恢复本地草稿')
      if (saved.content.includes('pending-video:')) ElMessage.warning('草稿中有尚未上传的视频占位符；浏览器未保存视频文件，请删除该占位行后重新插入视频。')
    } catch {
      localStorage.removeItem(draftStorageKey(uid))
    }
  } catch { /* Ignore malformed or inaccessible local drafts. */ }
}

function warnBeforeUnload(event: BeforeUnloadEvent) {
  if (!isEditing.value || (!isDirty.value && !videoUploading.value)) return
  event.preventDefault()
  event.returnValue = ''
}

function chooseVideo() { videoInput.value?.click() }
function onVideoSelected(event: Event) {
  const input = event.target as HTMLInputElement
  const files = Array.from(input.files ?? [])
  input.value = ''
  try { queueVideos(files) } catch (error) { ElMessage.warning(error instanceof Error ? error.message : '无法添加视频') }
}

function enterEditMode() {
  const guid = typeof props.id === 'string' ? props.id : ''
  if (guid) router.push(`/file/${guid}/edit`)
}

function leaveEditMode() {
  const guid = typeof props.id === 'string' ? props.id : ''
  if (guid) router.push(`/file/${guid}`)
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

async function onPublishStatusChanged(value: FileStatus) {
  const current = file.value
  if (!current?.uid || current.publishStatus === value) return

  try {
    publishing.value = true

    let dto: FileDto

    switch (value) {
      case FileStatus.Draft:
        dto = await new FileDraftWorkflow(current, fileApi, fileStore).run()
        break
      case FileStatus.Published:
        dto = await new FilePublishWorkflow(current, fileApi, fileStore).run()
        break
      case FileStatus.Scheduled:
        if (current.publishStatus === FileStatus.Published) {
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

  if (current.publishStatus === FileStatus.Draft) {
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
    await router.push('/file/list')
  } catch (error) {
    if (error === 'cancel' || error === 'close') return
    ElMessage.error(error instanceof Error ? error.message : '删除文章失败')
  } finally {
    deleting.value = false
  }
}

async function saveVersion() {
  if (!file.value?.uid || (!isDirty.value && !videoUploading.value) || loading.value) return

  loading.value = true

  try {
    await waitForVideoUploads()
    if (markdown.value.includes('pending-video:')) throw new Error('草稿中仍有未完成的视频占位符，请重新插入视频后再保存。')
    const resolvedMarkdown = markdown.value

    const fileName = `${file.value.fileName || '未命名'}${file.value.fileType || '.md'}`
    const contentFile = new File(
      [resolvedMarkdown],
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
    originalMarkdown.value = resolvedMarkdown
    if (file.value?.uid) localStorage.removeItem(draftStorageKey(file.value.uid))
    ElMessage.success('已保存为新版本')
    leaveEditMode()
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

  // Capture image pastes before Monaco handles the clipboard event.
  container.value?.addEventListener('paste', onEditorPaste, true)
  window.addEventListener('beforeunload', warnBeforeUnload)

  const guid = typeof props.id === 'string' ? props.id : ''
  if (guid) {
    await loadArticle(guid)
  }
})

watch(
  () => props.id,
  async guid => {
    if (guid) await loadArticle(guid)
  },
)

watch(markdown, value => {
  const uid = file.value?.uid
  if (!uid || !isEditing.value) return
  if (value === originalMarkdown.value) {
    localStorage.removeItem(draftStorageKey(uid))
    return
  }
  try {
    localStorage.setItem(draftStorageKey(uid), JSON.stringify({ content: value, savedAt: Date.now() }))
  } catch { /* Storage may be unavailable or full; the editor remains usable. */ }
}, { flush: 'sync' })

watch(isEditing, async value => {
  editor?.updateOptions({ readOnly: !value })
  if (value && file.value?.uid) await restoreLocalDraft(file.value.uid, originalMarkdown.value)
})

onBeforeUnmount(() => {
  container.value?.removeEventListener('paste', onEditorPaste, true)
  window.removeEventListener('beforeunload', warnBeforeUnload)
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
            popper-class="publish-schedule-picker-popper"
            :disabled="publishing || scheduling || file.publishStatus === FileStatus.Published"
            placeholder="预定发布时间"
            @change="onScheduledAtChanged"
          />
        </div>
        <template v-if="isEditing">
          <input ref="videoInput" type="file" accept=".mp4,.webm,video/mp4,video/webm" multiple hidden @change="onVideoSelected">
          <ElButton @click="chooseVideo">插入视频</ElButton>
          <ElButton :icon="Lock" @click="leaveEditMode">
            退出编辑
          </ElButton>
          <ElButton
            type="primary"
            :icon="Check"
            :disabled="(!isDirty && !videoUploading) || loading"
            @click="saveVersion"
          >
            保存版本
          </ElButton>
        </template>
        <template v-else>
          <ElButton :icon="Delete" :disabled="deleting || publishing || scheduling" @click="deleteArticle">
            删除
          </ElButton>
          <ElButton type="primary" :icon="Edit" @click="enterEditMode">
            编辑
          </ElButton>
        </template>
      </template>
    </MgsPageHeader>

    <div v-if="isEditing && videoUploads.length" class="video-upload-queue">
      <div v-for="item in videoUploads" :key="item.id" class="video-upload-item">
        <span class="video-upload-name">{{ item.name }}</span>
        <span>{{ item.state === 'queued' ? '排队中' : item.state === 'uploading' ? '上传中 ' + item.progress + '%' : item.state === 'completed' ? '已完成' : '上传失败' }}</span>
        <span v-if="item.error" class="video-upload-error">{{ item.error }}</span>
        <ElButton v-if="item.state === 'failed'" text type="primary" @click="retryVideoUpload(item.id)">重试</ElButton>
      </div>
    </div>
    <div class="editor-grid">
      <div ref="container" class="editor" v-loading="imageUploading" />
      <div class="render" v-loading="markdownRenderRef?.loading">
        <MarkdownRender ref="markdownRenderRef" :markdown="markdown" :baseurl="baseurl" :file-resolver="markdownFileResolver" />
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

.video-upload-queue {
  display: flex; flex-wrap: wrap; gap: 8px 16px; padding: 8px 16px;
  border-top: 1px solid var(--mgs-border); background: var(--mgs-surface-soft); font-size: 12px;
}
.video-upload-item { display: flex; align-items: center; gap: 8px; min-width: 240px; }
.video-upload-name { max-width: 220px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.video-upload-error { color: var(--el-color-danger); }

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

:global(.publish-schedule-picker-popper) {
  z-index: 4100 !important;
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
