<template>
  <ElCol class="dataview">
    <ElRow :gutter="20" style="padding:20px 0;">
      <ElCol :span="5">
        <ElInput v-model="searchText" :disabled="filterDisabled || !searchField" @keyup.enter="onSearchEnter">
          <template #prepend>
            <ElSelect v-model="searchField" placeholder="选择字段" style="width: 115px" :disabled="filterDisabled">
              <ElOption
                v-for="option in searchFieldOptions"
                :key="option.value"
                :label="option.label"
                :value="option.value"
              />
            </ElSelect>
          </template>
        </ElInput>
      </ElCol>
      <ElCol :span="5">
        <ElSelect
          v-model="selectedTags"
          multiple
          collapse-tags
          filterable
          placeholder="选择标签"
          :disabled="filterDisabled"
          style="width: 100%"
          value-key="uid"
        >
          <ElOption
            v-for="(tag, index) in tagOptions"
            :key="tag.uid ?? tag.name ?? index"
            :label="tag.name ?? ''"
            :value="tag"
          />
        </ElSelect>
      </ElCol>
      <ElCol :span="5">
        <ElSlider
          :step="10"
          range
          show-stops
          :max="100"
          v-model="sizeRange"
          style="width:inherit;"
          :disabled="filterDisabled"
        />
      </ElCol>
      <ElCol :span="5">
        <ElDatePicker
          type="daterange"
          range-separator="To"
          start-placeholder="Start Date"
          end-placeholder="End Date"
          value-format="YYYY-MM-DD"
          format="YYYY-MM-DD"
          v-model="dateRange"
          :disabled="filterDisabled"
        />
      </ElCol>
      <ElCol :span="3">
        <ElButton style="width: inherit;" :icon="Upload" @click="handleUploadFile" type="warning"/>
        <ElButton style="width: inherit;" :icon="Plus" @click="handleAddFile" type="success"/>
      </ElCol>
    </ElRow>
    <ElTable :data="tableData" highlight-current-row @row-click="activeRow" style="height:100%;">
      <ElTableColumn :prop="tableColumns[0]" :label="columnMap[tableColumns[0]]">
        <template #default="scope">
          <ElImage v-if="isImageType(scope.row.fileType)" class="image" :src="fileStore.previewUrl.get(scope.row.uid)">
            <template #error>加载中...</template>
          </ElImage>
          <ElImage v-else class="image" style="font-size: 5vw;">
            <template #error>
              <ElIcon>
                <component :is="mgsIconRegistry[markdown]" />
              </ElIcon>
            </template>
          </ElImage>
          <ElCol>{{ scope.row.uid }}</ElCol>
        </template>
      </ElTableColumn>
      <!-- <ElTableColumn :prop="tableColumns[0]" :label="columnMap[tableColumns[0]]" /> -->
      <ElTableColumn :prop="tableColumns[1]" :label="columnMap[tableColumns[1]]" />
      <ElTableColumn :prop="tableColumns[2]" :label="columnMap[tableColumns[2]]">
        <template #default="scope">
          <template v-if="scope.row.tags.length > 0 && scope.row.tags != undefined">
            <ElTag v-for="(tag, tagKey) in scope.row.tags" :key="tagKey" class="gap">{{ tag.name }}</ElTag>
          </template>
          <ElTag v-else>
            <ElIcon>
              <Plus />
            </ElIcon>
          </ElTag>
        </template>

      </ElTableColumn>
      <ElTableColumn :prop="tableColumns[3]" :label="columnMap[tableColumns[3]]" sortable>
        <template #default="scope">
          <ElText v-if="scope.row.fileSize >= 1024">{{ (scope.row.fileSize as number / 1024).toFixed(3) }} KB</ElText>
          <ElText v-else-if="scope.row.fileSize < 1024">{{ scope.row.fileSize }} B</ElText>
        </template>
      </ElTableColumn>
      <ElTableColumn :prop="tableColumns[4]" :label="columnMap[tableColumns[4]]" />
      <ElTableColumn :prop="tableColumns[5]" :label="columnMap[tableColumns[5]]" sortable />
      <ElTableColumn :prop="tableColumns[6]" :label="columnMap[tableColumns[6]]" :filters="extensionFilters"
        :filter-method="handleFilter" />

      </ElTable>

    <FileUpload v-model:file-list="UploadContext.fileList"
      v-model:tags="UploadContext.tags" />
    <FileTool  :initial-tags="activeDto.tags!"
      v-model:model-value="activeDto" />
    <!-- 防止tag修改渗透,仅允许在update事件成功以后,由update回调至fileDto -->
  </ElCol>
</template>

<script setup lang="ts">
import { isImageType } from '@aqlife/domain'
import { MgsIconName, mgsIconRegistry } from '@aqlife/icons'
import {
  ElTable,
  ElSlider,
  ElDatePicker,
  ElTag,
  ElText,
  ElTableColumn,
  ElImage,
  ElInput,
  ElSelect,
  ElOption,
  ElButton,
  ElIcon,
  type UploadUserFile,
} from 'element-plus'
import { FileApi, type FileDto, type TagDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { onBeforeMount, ref, reactive, computed, onBeforeUnmount } from 'vue'
import { useFileStore } from '@/stores/useFileStore'
import { useActionStore, OperationalState } from '@/stores/useActionStore'
import FileUpload from '@/components/FileUpload.vue'
import FileTool from '@/components/FileTool.vue'
import { Plus,Upload } from '@element-plus/icons-vue'
import { defaultFilePolicy } from '@aqlife/domain'

// 子组件参数
const searchField = ref<string | null>(null)
const searchText = ref<string>('')
const searchTerm = ref<string>('')
const selectedTags = ref<TagDto[]>([])
const sizeRange = ref<number[]>([0, 100])
const dateRange = ref<string[] | null>(null)
const markdown = MgsIconName.Markdown
const UploadContext = reactive<{ fileList: UploadUserFile[]; tags: TagDto[] }>({
  fileList: [],
  tags: [],
})
// ---组件属性
const fileApi = new FileApi(apiConfiguration)
const fileStore = useFileStore()
const actionStore = useActionStore()
const activeDto = ref<FileDto>({})

const extensionFilters = defaultFilePolicy.allowedUpload.map(ext => ({
  text: ext.toUpperCase(),
  value: ext
}))
const handleFilter = (value: string, row: any, column: any) => {
  const property = column['property']
  // 确保这里的判断逻辑与你后端 FileMetaEntity 的 Extension 字段对齐 [cite: 9]
  return row[property] === value
}
const activeRow = (row: any) => {
  actionStore.OState = OperationalState.Update
  activeDto.value = row as FileDto
  // drawerStatus.value = !drawerStatus.value
}

const tableColumns = computed(() => {
  if (!fileStore.fileList || fileStore.fileList.length === 0) {
    return []
  }
  return Object.keys(fileStore.fileList[0])
})

const searchFieldOptions = computed(() =>
  tableColumns.value.slice(0, 2).map(column => ({
    label: columnMap[column] ?? column,
    value: column,
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
    rowTags.some((tag:TagDto) => tag?.uid && selected.uid && tag.uid === selected.uid),
  )
}

function matchesSize(row: FileDto) {
  if (sizeRange.value[0] === 0 && sizeRange.value[1] === 100) return true
  const fileSizeKb = (row.fileSize ?? 0) / 1024
  return fileSizeKb >= sizeRange.value[0] && fileSizeKb <= sizeRange.value[1]
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
    matchesSearch(item) && matchesTags(item) && matchesSize(item) && matchesDate(item),
  ) ?? [],
)

const tableData = computed(() => (filterDisabled.value ? [] : filteredFileList.value))

// 3. 映射表：把英文 key 转换成中文表头（非必须，如果不配置则默认显示 key 名字）
const columnMap: Record<string, string> = {
  uid: 'ID & 预览',
  fileName: '文件名',
  fileSize: '文件大小',
  fileHash: '文件哈希',
  uploadTime: '上传时间',
  fileType: '文件类型',
  tags: '标签',
}

onBeforeMount(async () => {
  await fileStore.fetchAllFiles(fileApi)
  // actionStore.onAdd = handleAddFile
  activeDto.value = { fileName: '', fileHash: '', fileType: '', fileSize: 0 }
})

function handleUploadFile() {
  actionStore.OState = OperationalState.Upload
  console.log(actionStore.OState)
}

function handleAddFile(){
  actionStore.OState = OperationalState.Add
}

// onBeforeUnmount(() => actionStore.resetActions())
</script>

<style lang="css" scoped>
.image {
  width: 5vw;
  height: 5vw;
  /* font-size: 5vw; */
}

.gap {
  margin-right: 10px;
}

.dataview {
  height: inherit;
  overflow-y: scroll;
}

.dataview::-webkit-scrollbar {
  display: none;
}
</style>
