<template>
  <ElTable :data="fileStore.fileList" highlight-current-row @row-click="activeRow">
    <!-- <ElTableColumn sortable :prop="item" v-for="(item, index) in tableColumns" :key="item" :label="columnMap[item] ?? item" :filters="index==(tableColumns.length-1)?extensionFilters:undefined" :filter-method="index === tableColumns.length - 1 ? handleFilter : undefined">

      <template #default="scope" v-if="index ==0">
        <ElImage v-if="isImageType(scope.row.fileType)" class="image" :src="fileStore.previewUrl.get(scope.row.uid)">
          <template #error>加载中...</template>
</ElImage>

<ElImage src="" v-else class="image" style="font-size: 5vw;">
  <template #error>
            <ElIcon><component :is="mgsIconRegistry[markdown]"/></ElIcon>
          </template>
</ElImage>
</template>

<template #default="scope" v-else-if="index==2">
        <template v-if="scope.row.tags.length > 0 && scope.row.tags != undefined">
          <ElTag v-for="(tag, tagKey) in scope.row.tags" :key="tagKey" class="gap">{{tag.name}}</ElTag>
        </template>

<ElTag>
  <ElIcon>
    <Plus />
  </ElIcon>
</ElTag>
</template>
</ElTableColumn> -->
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
        <ElText v-if="scope.row.fileSize>=1024">{{ (scope.row.fileSize as number /1024).toFixed(3) }} KB</ElText>
        <ElText v-else-if="scope.row.fileSize<1024">{{ scope.row.fileSize }} B</ElText>
      </template>
    </ElTableColumn>
    <ElTableColumn :prop="tableColumns[4]" :label="columnMap[tableColumns[4]]" />
    <ElTableColumn :prop="tableColumns[5]" :label="columnMap[tableColumns[5]]" sortable/>
    <ElTableColumn :prop="tableColumns[6]" :label="columnMap[tableColumns[6]]" :filters="extensionFilters" :filter-method="handleFilter"/>

  </ElTable>

  <FileUpload v-if="actionStore.OState == OperationalState.Add" v-model:file-list="UploadContext.fileList"
    v-model:tags="UploadContext.tags" />
    <FileTool v-else-if="actionStore.OState == OperationalState.Update" :initial-tags="activeDto.tags!" v-model:model-value="activeDto" />
    <!-- 防止tag修改渗透,仅允许在update事件成功以后,由update回调至fileDto -->
</template>

<script setup lang="ts">
import { isImageType } from '@aqlife/domain'
import { MgsIconName, mgsIconRegistry } from '@aqlife/icons'
import { routerAction } from '@/stores/useActionStore'
import {
  ElTable,
  ElTag,
  ElText,
  ElTableColumn,
  ElImage,
  ElDrawer,
  ElLoading,
  type TableInstance,
  ElMessage,
  type UploadUserFile,
  type UploadRawFile,
  type UploadFile,
} from 'element-plus'
import { FileApi, type FileMetadataDto, type TagDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { onBeforeMount, ref, reactive, computed, onBeforeUnmount } from 'vue'
import { useFileStore } from '@/stores/useFileStore'
import { useActionStore, OperationalState } from '@/stores/useActionStore'
import FileUpload from '@/components/FileUpload.vue'
import FileTool from '@/components/FileTool.vue'
import { Plus } from '@element-plus/icons-vue'
import { defaultFilePolicy } from '@aqlife/domain'

// 子组件参数

const markdown = MgsIconName.Markdown
const UploadContext = reactive<{ fileList: UploadUserFile[]; tags: TagDto[] }>({
  fileList: [],
  tags: [],
})
// ---组件属性
const fileApi = new FileApi(apiConfiguration)
const fileStore = useFileStore()
const actionStore = useActionStore()
const activeDto = ref<FileMetadataDto>({})

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
  activeDto.value = row as FileMetadataDto
  // drawerStatus.value = !drawerStatus.value
}

// const drawerStatus = ref(false)


const tableColumns = computed(() => {
  if (!fileStore.fileList || fileStore.fileList.length === 0) {
    return []
  }
  return Object.keys(fileStore.fileList[0])
})

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
  actionStore.onAdd = handleAddFile
  activeDto.value = { fileName: '', fileHash: '', fileType: '', fileSize: 0 }
})

function handleAddFile() {
  actionStore.OState = OperationalState.Add
  // drawerStatus.value = !drawerStatus.value
}

onBeforeUnmount(() => actionStore.resetActions())
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
</style>
