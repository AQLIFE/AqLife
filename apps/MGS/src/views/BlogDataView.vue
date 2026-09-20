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
      <ElTableColumn v-for="column in tableColumns" :key="column.prop" :prop="column.prop" :label="column.label"
      :width="column.width" :sortable="column.sortable" :filters="column.filter?.options.map(option=>({text:option.label,value:option.value.toString() }))" :filter-method="column.filter?(value, row) => `${row[column.prop]}` === value:undefined" :filter-multiple="column.filter?.multiple ?? false"
>
        <template #default="{row}">
          <component v-if="column.renderer" :is="column.renderer" :row="row" :value="row[column.prop]"/>
          <template v-else>{{ row[column.prop] }}</template>
        </template>
      </ElTableColumn>
      </ElTable>

    <FileUpload v-model:file-list="UploadContext.fileList" v-model:tags="UploadContext.tags" />
    <FileTool  :initial-tags="activeDto.tags!" v-model:model-value="activeDto" />
    <!-- 防止tag修改渗透,仅允许在update事件成功以后,由update回调至fileDto -->
  </ElCol>
</template>

<script setup lang="ts">
import {
  ElTable,
  ElSlider,
  ElDatePicker,
  ElTableColumn,
  ElInput,
  ElSelect,
  ElOption,
  ElButton,
  type UploadUserFile,
} from 'element-plus'
import { FileApi, type FileDto, type TagDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { onBeforeMount, ref, reactive, computed, type Component } from 'vue'
import { useFileStore } from '@/stores/useFileStore'
import { useActionStore, OperationalState } from '@/stores/useActionStore'
import FileUpload from '@/components/FileUpload.vue'
import FileTool from '@/components/FileTool.vue'
import { Plus,Upload } from '@element-plus/icons-vue'
import { useRouter } from 'vue-router'
import FilePreviewCell from '@/components/FilePreviewCell.vue'
import TagsCell from '@/components/TagsCell.vue'
import FileSizeCell from './FileSizeCell.vue'
import PublishStatusCell from './PublishStatusCell.vue'
import { publishStatusOptions, type TableFilterOption } from '@/types/TableFilterOption.ts'

// 组件核心
type FileTableColumn = {
  prop: keyof FileDto
  label: string,
  renderer?:Component,
  sortable?: boolean,
  width?:number|string,
  fixed?:'left'|'right',
  filter?: {
    options: TableFilterOption[]
    multiple?: boolean
  }
}


const tableColumns: FileTableColumn[] = [
  {
    prop: 'uid',
    label: 'ID & 预览',
    renderer:FilePreviewCell,
    width:'130',
    fixed:'left'
  },
  {
    prop: 'fileName',
    label: '文件名',
    width:200
  },
  {
    prop: 'tags',
    label: '标签',
    renderer:TagsCell,
    width:380
  },
  {
    prop: 'publishStatus',
    label: '发布状态',
    renderer:PublishStatusCell,
    width:120,
    sortable:true,
    filter:{options:publishStatusOptions,multiple:true}
  },
  {
    prop: 'fileSize',
    label: '文件大小',
    renderer: FileSizeCell,
    sortable: true,
    width:120
  },
  {
    prop: 'uploadTime',
    label: '上传时间',
    width:120,
    sortable:true
  },
  {
    prop: 'fileHash',
    label: '文件哈希',
  },
]

// const adatar = computed(()=>{
//   return column.filter?.options.map(option=>({text:option.label,value:option.value}))
// })

// 检索参数
const searchField = ref<keyof FileDto | null>(null)
const searchText = ref<string>('')
const searchTerm = ref<string>('')
const selectedTags = ref<TagDto[]>([])
const sizeRange = ref<number[]>([0, 100])
const dateRange = ref<string[] | null>(null)

const UploadContext = reactive<{ fileList: UploadUserFile[]; tags: TagDto[] }>({
  fileList: [],
  tags: [],
})
// ---组件属性
const fileApi = new FileApi(apiConfiguration)
const fileStore = useFileStore()
const actionStore = useActionStore()
const activeDto = ref<FileDto>({})

// const extensionFilters = defaultFilePolicy.allowedUpload.map(ext => ({
//   text: ext.toUpperCase(),
//   value: ext
// }))
// const handleFilter = (value: any, row: any, column: TableColumnCtx<FileDto>) => {
//   const property = column['property']
//   // 确保这里的判断逻辑与你后端 FileMetaEntity 的 Extension 字段对齐 [cite: 9]
//   return row[property] === value
// }
// function handleFilter(column: FileTableColumn) {
//   return (
//     value: unknown,
//     row: FileDto
//   ) => {
//     return row[column.prop] === value
//   }
// }
const activeRow = (row: FileDto) => {
  actionStore.OState = OperationalState.Update
  activeDto.value = row
  // drawerStatus.value = !drawerStatus.value
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


onBeforeMount(async () => {
  await fileStore.fetchAllFiles(fileApi)
  // actionStore.onAdd = handleAddFile
  activeDto.value = { fileName: '', fileHash: '', fileType: '', fileSize: 0 }
})

function handleUploadFile() {
  actionStore.OState = OperationalState.Upload
}
const router = useRouter()
function handleAddFile(){
  actionStore.OState = OperationalState.Add

  router.push('/blog/view')
}

</script>

<style lang="css" scoped>
.image {
  width: 5vw;
  height: 5vw;
  /* font-size: 5vw; */
}

.dataview {
  height: inherit;
  overflow-y: scroll;
}

.dataview::-webkit-scrollbar {
  display: none;
}
</style>
