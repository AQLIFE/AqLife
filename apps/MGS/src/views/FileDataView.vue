<template>
  <div class="dataview">
    <MgsPageHeader title="文件" description="管理文件, 编写草稿和发布文件"><template #actions>
        <ElButton :icon="Upload" @click="handleUploadFile">Upload</ElButton>
        <ElButton type="primary" :icon="Plus" @click="handleAddFile">New article</ElButton>
      </template>
    </MgsPageHeader>
    <MgsToolbar>
      <ElSegmented v-model="selectMode" :options="segmentedMode"/>

      <ElInput v-model="searchText" :disabled="filterDisabled || !searchField" @keyup.enter="onSearchEnter"
        placeholder="Search..." class="toolbar-control"><template #prepend>
          <ElSelect v-model="searchField" placeholder="Field" style="width:100px">
            <ElOption v-for="option in searchFieldOptions" :key="option.value" :label="option.label"
              :value="option.value" />
          </ElSelect>
        </template>
      </ElInput>

      <ElSelect v-model="selectedTags" multiple collapse-tags filterable placeholder="Tags" :disabled="filterDisabled"
        class="toolbar-control" value-key="uid">
        <ElOption v-for="(tag, index) in tagOptions" :key="tag.uid ?? tag.name ?? index" :label="tag.name ?? ''"
          :value="tag" />
      </ElSelect>

      <ElDatePicker type="daterange" range-separator="to" start-placeholder="Start" end-placeholder="End"
        value-format="YYYY-MM-DD" format="YYYY-MM-DD" v-model="dateRange" :disabled="filterDisabled" />
    </MgsToolbar>

    <ElTable :data="tableData" v-loading="isLoading" element-loading-text="正在加载文件..." empty-text="暂无文件" highlight-current-row @row-click="activeRow" height="100%" class="data-table">
      <ElTableColumn v-for="column in tableColumns" :key="column.prop" :prop="column.prop" :label="column.label"
        :width="column.width" :sortable="column.sortable"
        :filters="column.filter?.options.map(option => ({ text: option.label, value: option.value.toString() }))"
        :filter-method="column.filter ? (value, row) => `${row[column.prop]}` === value : undefined"
        :filter-multiple="column.filter?.multiple ?? false">
        <template #default="{ row }">
          <FilePreviewCell
            v-if="column.renderer === FilePreviewCell"
            :row="row"
            :value="row[column.prop]"
            @preview-image="openImagePreview"
            @preview-video="openVideoPreview"
          />
          <component
            v-else-if="column.renderer"
            :is="column.renderer"
            :row="row"
            :value="row[column.prop]"
          />
          <template v-else>{{ row[column.prop] }}</template>
        </template>
      </ElTableColumn>
    </ElTable>

    <div
      v-if="previewImageUrl || previewVideoUrl"
      class="image-preview-mask"
      role="presentation"
      @click="closeImagePreview"
    >
      <img v-if="previewImageUrl" class="image-preview-content" :src="previewImageUrl" alt="图片预览">
      <video v-else class="video-preview-content" :src="previewVideoUrl" controls autoplay playsinline preload="metadata" @click.stop />
      <section v-if="activeImageFile" class="image-management-panel" @click.stop>
        <div class="image-management-heading">
          <strong>{{ activeImageFile.fileName || '文件管理' }}</strong>
          <ElButton text @click="closeImagePreview">关闭预览</ElButton>
        </div>
        <label class="management-label">标签</label>
        <TagSelect
          v-model:tag-list="imageSelectedTags"
          :select-disabled="imageSaving || imageDeleting"
          popper-class="image-tag-select-popper"
          @update:tag-list="onImageTagsChanged"
        />
        <label class="management-label">发布状态</label>
        <ElSegmented
          :model-value="imagePublishStatus"
          :options="imagePublishOptions"
          :disabled="imageSaving || imageDeleting"
          @change="onImagePublishStatusChanged"
        />
        <ElDatePicker
          v-model="imageScheduledAt"
          type="datetime"
          class="image-schedule-picker"
          placeholder="预定发布时间"
          :disabled="imageSaving || imageDeleting || activeImageFile.publishStatus === FileStatus.Published"
          @change="onImageScheduledAtChanged"
        />
        <ElButton
          type="danger"
          :icon="Delete"
          :loading="imageDeleting"
          :disabled="imageSaving"
          @click="deleteImage"
        >删除文件</ElButton>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import {
  ElTable,
  ElDatePicker,
  ElMessage,
  ElMessageBox,
  ElTableColumn,
  ElInput,
  ElSelect,
  ElOption,
  ElButton,
  ElSegmented
} from 'element-plus'
import { FileApi, FileScope, FileStatus, type FileDto, type TagDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { tokenStore } from '@aqlife/api-client'
import { onBeforeMount, ref, computed, watch, type Component } from 'vue'
import { useFileStore } from '@/stores/useFileStore'
import { Delete, Plus, Upload } from '@element-plus/icons-vue'
import MgsPageHeader from '@/components/ui/MgsPageHeader.vue'
import MgsToolbar from '@/components/ui/MgsToolbar.vue'
import { useRouter } from 'vue-router'
import FilePreviewCell from '@/components/FilePreviewCell.vue'
import TagsCell from '@/components/TagsCell.vue'
import FileSizeCell from './FileSizeCell.vue'
import PublishStatusCell from './PublishStatusCell.vue'
import TagSelect from '@/components/TagSelect.vue'
import { FileDeleteWorkflow, FileDraftWorkflow, FilePublishWorkflow, FileScheduledWorkflow, FileTagUpdateWorkflow } from '@/workflow'
import { publishStatusOptions, type TableFilterOption } from '@/types/TableFilterOption.ts'

// 组件核心
type FileTableColumn = {
  prop: keyof FileDto
  label: string,
  renderer?: Component,
  sortable?: boolean,
  width?: number | string,
  filter?: {
    options: TableFilterOption[]
    multiple?: boolean
  }
}

const segmentedMode = [
  { label: 'Markdown', value: 'md' },
  { label: 'Images', value: 'image' },
  { label: 'Videos', value: 'video' },
]
const selectMode = ref<string>('md')
const previewImageUrl = ref('')
const previewVideoUrl = ref('')
const videoPreviewLoading = ref(false)
let videoPreviewObjectUrl: string | null = null
let videoPreviewRequestVersion = 0
const activeImageFile = ref<FileDto | null>(null)
const imageSelectedTags = ref<TagDto[]>([])
const imagePublishStatus = ref<FileStatus>(FileStatus.Draft)
const imageScheduledAt = ref<Date | null>(null)
const imageSaving = ref(false)
const imageDeleting = ref(false)
const isLoading = ref(false)
let modeLoadVersion = 0
const tableColumns: FileTableColumn[] = [
  {
    prop: 'uid',
    label: 'ID & 预览',
    renderer: FilePreviewCell,
    width: '130',
  },
  {
    prop: 'fileName',
    label: '文件名',
    width: 200
  },
  {
    prop: 'tags',
    label: '标签',
    renderer: TagsCell,
    width: 350
  },
  {
    prop: 'publishStatus',
    label: '发布状态',
    renderer: PublishStatusCell,
    width: 120,
    sortable: true,
    filter: { options: publishStatusOptions, multiple: true }
  },
  {
    prop: 'fileSize',
    label: '文件大小',
    renderer: FileSizeCell,
    sortable: true,
    width: 120
  },
  {
    prop: 'uploadTime',
    label: '上传时间',
    width: 120,
    sortable: true
  },
  {
    prop: 'fileHash',
    label: '文件哈希',
  },
]

// 检索参数
const searchField = ref<keyof FileDto | null>(null)
const searchText = ref<string>('')
const searchTerm = ref<string>('')
const selectedTags = ref<TagDto[]>([])
const dateRange = ref<string[] | null>(null)

// ---组件属性
const fileApi = new FileApi(apiConfiguration)
const fileStore = useFileStore()
const router = useRouter()

const imagePublishOptions = computed(() =>
  publishStatusOptions.map(item => ({
    ...item,
    disabled:
      item.value === FileStatus.Scheduled &&
      activeImageFile.value?.publishStatus === FileStatus.Published,
  })),
)

const activeRow = (row: FileDto) => {
  // In image mode, clicking a row should not navigate to the Markdown editor.
  if (selectMode.value === 'image' || selectMode.value === 'video' || !row.uid) return
  router.push(`/file/${row.uid}`)
}

function parseImagePublishAt(value: FileDto['publishAt']): Date | null {
  if (!value) return null
  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? null : date
}

function openImagePreview(uid: string) {
  const url = fileStore.previewUrl.get(uid)
  const file = fileStore.fileList.find(item => item.uid === uid)
  if (!url || !file) return

  activeImageFile.value = file
  previewImageUrl.value = url
  imageSelectedTags.value = file.tags ?? []
  imagePublishStatus.value = file.publishStatus ?? FileStatus.Draft
  imageScheduledAt.value = parseImagePublishAt(file.publishAt)
}

async function openVideoPreview(uid: string) {
  const file = fileStore.fileList.find(item => item.uid === uid)
  if (!file?.uid) return

  const requestVersion = ++videoPreviewRequestVersion
  const basePath = apiConfiguration.basePath.replace(/\/+$/, '')
  const token = tokenStore.get()
  previewImageUrl.value = ''
  previewVideoUrl.value = ''
  if (videoPreviewObjectUrl) {
    URL.revokeObjectURL(videoPreviewObjectUrl)
    videoPreviewObjectUrl = null
  }
  activeImageFile.value = file
  imageSelectedTags.value = file.tags ?? []
  imagePublishStatus.value = file.publishStatus ?? FileStatus.Draft
  imageScheduledAt.value = parseImagePublishAt(file.publishAt)
  videoPreviewLoading.value = true

  try {
    const headers = new Headers()
    if (token) headers.set('Authorization', token.startsWith('Bearer ') ? token : `Bearer ${token}`)
    const response = await fetch(
      `${basePath}/api/File/preview?UID=${encodeURIComponent(uid)}`,
      { method: 'GET', headers, credentials: 'include' },
    )

    if (requestVersion !== videoPreviewRequestVersion) return
    if (!response.ok) {
      if ([401, 403, 404].includes(response.status)) {
        ElMessage.warning('该视频未发布或当前账号无权观看')
      } else {
        ElMessage.error(`视频预览失败（HTTP ${response.status}）`)
      }
      activeImageFile.value = null
      return
    }

    const blob = await response.blob()
    if (requestVersion !== videoPreviewRequestVersion) return
    videoPreviewObjectUrl = URL.createObjectURL(blob)
    previewVideoUrl.value = videoPreviewObjectUrl
  } catch (error) {
    if (requestVersion === videoPreviewRequestVersion) {
      ElMessage.error(error instanceof Error ? `视频预览失败：${error.message}` : '视频预览请求失败')
      activeImageFile.value = null
    }
  } finally {
    if (requestVersion === videoPreviewRequestVersion) videoPreviewLoading.value = false
  }
}

function closeImagePreview() {
  videoPreviewRequestVersion++
  previewImageUrl.value = ''
  previewVideoUrl.value = ''
  videoPreviewLoading.value = false
  if (videoPreviewObjectUrl) {
    URL.revokeObjectURL(videoPreviewObjectUrl)
    videoPreviewObjectUrl = null
  }
  activeImageFile.value = null
}

async function onImageTagsChanged(value: FileDto['tags']) {
  const current = activeImageFile.value
  if (!current?.uid || !value) return

  try {
    imageSaving.value = true
    const updated = await new FileTagUpdateWorkflow(current, value, fileApi, fileStore).run()
    activeImageFile.value = updated
    imageSelectedTags.value = updated.tags ?? []
    ElMessage.success('文件标签已更新')
  } catch (error) {
    imageSelectedTags.value = current.tags ?? []
    ElMessage.error(error instanceof Error ? error.message : '文件标签更新失败')
  } finally {
    imageSaving.value = false
  }
}

async function onImagePublishStatusChanged(value: FileStatus) {
  const current = activeImageFile.value
  if (!current?.uid || current.publishStatus === value) return

  try {
    imageSaving.value = true
    let updated: FileDto

    switch (value) {
      case FileStatus.Draft:
        updated = await new FileDraftWorkflow(current, fileApi, fileStore).run()
        break
      case FileStatus.Published:
        updated = await new FilePublishWorkflow(current, fileApi, fileStore).run()
        break
      case FileStatus.Scheduled:
        if (current.publishStatus === FileStatus.Published) {
          throw new Error('禁止从已发布状态修改为预定发布状态')
        }
        if (!current.publishAt) {
          ElMessage.warning('请先设置预定发布时间')
          return
        }
        updated = await new FileScheduledWorkflow(current, fileApi, fileStore).run()
        break
      default:
        return
    }

    activeImageFile.value = updated
    imagePublishStatus.value = updated.publishStatus ?? FileStatus.Draft
    imageScheduledAt.value = parseImagePublishAt(updated.publishAt)
    ElMessage.success('文件发布状态已更新')
  } catch (error) {
    imagePublishStatus.value = current.publishStatus ?? FileStatus.Draft
    ElMessage.error(error instanceof Error ? error.message : '图片发布状态更新失败')
  } finally {
    imageSaving.value = false
  }
}

async function onImageScheduledAtChanged(value: Date | null) {
  const current = activeImageFile.value
  if (!current?.uid || !value) return

  const previousPublishAt = current.publishAt
  current.publishAt = value.toISOString()
  imageScheduledAt.value = value

  if (current.publishStatus === FileStatus.Draft) {
    fileStore.upsertFile(current)
    ElMessage.success('预定发布时间已设置')
    return
  }

  try {
    imageSaving.value = true
    const updated = await new FileScheduledWorkflow(current, fileApi, fileStore).run()
    activeImageFile.value = updated
    imagePublishStatus.value = updated.publishStatus ?? FileStatus.Draft
    imageScheduledAt.value = parseImagePublishAt(updated.publishAt)
    ElMessage.success('预定发布时间已更新')
  } catch (error) {
    current.publishAt = previousPublishAt
    imageScheduledAt.value = parseImagePublishAt(previousPublishAt)
    ElMessage.error(error instanceof Error ? error.message : '设置预定发布时间失败')
  } finally {
    imageSaving.value = false
  }
}

async function deleteImage() {
  const current = activeImageFile.value
  if (!current?.uid || imageDeleting.value) return

  // 先关闭预览遮罩，再显示确认框，避免确认框被预览层遮挡。
  closeImagePreview()

  try {
    await ElMessageBox.confirm(
      `确定删除文件「${current.fileName || '未命名文件'}」吗？删除后将无法恢复。`,
      '删除文件',
      {
        confirmButtonText: '删除',
        cancelButtonText: '取消',
        type: 'warning',
      },
    )

    imageDeleting.value = true
    await new FileDeleteWorkflow(current, fileApi, fileStore).run()
    ElMessage.success('文件已删除')
  } catch (error) {
    if (error === 'cancel' || error === 'close') return
    ElMessage.error(error instanceof Error ? error.message : '删除文件失败')
  } finally {
    imageDeleting.value = false
  }
}



const searchFieldOptions = computed(() =>
  tableColumns.slice(0, 2).map(column => ({
    label: column.label,
    value: column.prop,
  })),
)

const filterDisabled = computed(() => !fileStore.fileList || fileStore.fileList.length === 0)

const tagOptions = computed<TagDto[]>(() => {
  const map = new Map<string, TagDto>()
  fileStore.fileList?.forEach((item: FileDto) => {
    item.tags?.forEach((tag: TagDto) => {
      if (!tag) return
      const key = tag.uid ?? tag.name ?? JSON.stringify(tag)
      if (!map.has(key)) {
        map.set(key, tag)
      }
    })
  })
  return Array.from(map.values())
})

function normalizeDateValue(value: string | Date | undefined | null) {
  if (!value) return ''
  if (typeof value === 'string') {
    return value.trim().slice(0, 10)
  }
  return value.toISOString().slice(0, 10)
}

function normalizeUploadTime(value: string | null | undefined) {
  if (!value) return ''
  return value.trim().slice(0, 10)
}

function onSearchEnter() {
  if (filterDisabled.value || !searchField.value) return
  searchTerm.value = searchText.value.trim()
}

function matchesSearch(row: FileDto) {
  if (!searchField.value || !searchTerm.value) return true
  const rawValue = (row as any)[searchField.value]
  const textValue = rawValue == null ? '' : String(rawValue)
  return textValue.toLowerCase().includes(searchTerm.value.toLowerCase())
}

function matchesTags(row: FileDto) {
  if (!selectedTags.value.length) return true
  const rowTags = row.tags ?? []
  return selectedTags.value.every(selected =>
    rowTags.some((tag: TagDto) => tag?.uid && selected.uid && tag.uid === selected.uid),
  )
}

function matchesDate(row: FileDto) {
  if (!dateRange.value || dateRange.value.length !== 2) return true
  const [start, end] = dateRange.value
  if (!start || !end) return true
  const startDate = normalizeDateValue(start)
  const endDate = normalizeDateValue(end)
  if (!startDate || !endDate) return true
  const uploadDate = normalizeUploadTime(row.uploadTime)
  if (!uploadDate) return false
  return uploadDate >= startDate && uploadDate <= endDate
}

const filteredFileList = computed(() =>
  fileStore.fileList?.filter((item: FileDto) =>
    matchesSearch(item) && matchesTags(item) && matchesDate(item),
  ) ?? [],
)

const tableData = computed(() => (filterDisabled.value ? [] : filteredFileList.value))


async function loadFilesForMode() {
  const version = ++modeLoadVersion
  const scope = selectMode.value === 'image'
    ? FileScope.Image
    : selectMode.value === 'video'
      ? FileScope.Video
      : FileScope.Blog

  isLoading.value = true
  fileStore.clearCache()

  try {
    await fileStore.fetchAllFiles(fileApi, scope)
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '文件列表加载失败')
  } finally {
    if (version === modeLoadVersion) isLoading.value = false
  }
}

onBeforeMount(loadFilesForMode)

watch(selectMode, async () => {
  searchText.value = ''
  searchTerm.value = ''
  selectedTags.value = []
  dateRange.value = null
  await loadFilesForMode()
})

function handleUploadFile() {
  router.push(selectMode.value === 'video' ? '/file/upload?type=video' : '/file/upload')
}
function handleAddFile() {
  router.push('/file/new')
}

</script>

<style lang="css" scoped>
.dataview {
  height: 100%;
  min-height: 0;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.data-table {
  width: 100%;
  flex: 1;
  min-height: 0;
}

.toolbar-control {
  max-width: 260px;
}

.image-preview-mask {
  position: fixed;
  inset: 0;
  z-index: 3000;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 32px;
  background: rgb(0 0 0 / 78%);
  cursor: zoom-out;
}

.image-preview-content,
.video-preview-content {
  display: block;
  max-width: min(70vw, 1100px);
  max-height: 88vh;
  object-fit: contain;
  box-shadow: 0 8px 32px rgb(0 0 0 / 35%);
}

.video-preview-content {
  width: min(70vw, 1100px);
  background: #000;
  cursor: default;
}

:global(.image-tag-select-popper) {
  z-index: 4001 !important;
}

.image-management-panel {
  position: fixed;
  top: 24px;
  right: 24px;
  bottom: 24px;
  display: flex;
  flex-direction: column;
  gap: 12px;
  width: min(340px, calc(100vw - 48px));
  padding: 20px;
  overflow-y: auto;
  color: var(--el-text-color-primary);
  background: var(--el-bg-color);
  border-radius: 12px;
  box-shadow: 0 8px 32px rgb(0 0 0 / 30%);
  cursor: default;
}

.image-management-heading {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
}

.image-management-heading strong {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.management-label {
  font-size: 13px;
  color: var(--el-text-color-secondary);
}

.image-schedule-picker {
  width: 100%;
}

@media (max-width: 700px) {
  .image-preview-mask {
    align-items: flex-start;
    padding: 16px;
    overflow-y: auto;
  }

  .image-preview-content,
  .video-preview-content {
    width: 100%;
    max-width: 100%;
    max-height: 45vh;
    margin-top: 8px;
  }

  .image-management-panel {
    position: fixed;
    top: auto;
    right: 16px;
    bottom: 16px;
    left: 16px;
    width: auto;
    max-height: 48vh;
  }
}
</style>
