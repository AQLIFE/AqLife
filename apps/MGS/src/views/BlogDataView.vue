<template>
  <ElTable :data="fileStore.fileList"  highlight-current-row @row-click="activeRow">
    <ElTableColumn
      sortable
      :prop="item"
      v-for="(item, index) in tableColumns"
      :key="item"
      :label="columnMap[item] ?? item"
    >
      <template #default="scope" v-if="index == 0">
        <ElImage :src="fileStore.previewUrl.get(scope.row.uid)">
          <template #error>加载中...</template>
        </ElImage>
      </template>
      <template #default="scope" v-else-if="index == 2">
        <!-- <ElTag >scope.row.tags</ElTag> -->
        <template v-if="scope.row.tags.length>0 && scope.row.tags!=undefined">
          <ElTag v-for="tag,tagKey in scope.row.tags" :key="tagKey">{{ tag }}</ElTag>
        </template>
        <ElTag ><ElIcon><Plus/></ElIcon></ElTag>
        <!-- <ElText v-else type="info">无标签</ElText> -->
      </template>
    </ElTableColumn>
  </ElTable>
  <ElDrawer v-model="drawerStatus" title="文件" with-header :show-close="false">
    <FileUpload :column-map="descriptionsMap" :file-dto="activeDto"/>
    <template #footer>
      <ElButton type="warning">commit</ElButton>
    </template>
  </ElDrawer>
</template>

<script setup lang="ts">
import { ElTable,ElTag,ElText, ElTableColumn, ElImage, ElDrawer,type TableInstance, ElMessage } from 'element-plus'
import { FileApi, type FileMetadataDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { onBeforeMount, ref, computed, onBeforeUnmount } from 'vue'
import AuthImage from '@/components/AuthImage.vue'
import { useFileStore } from '@/stores/useFileStore'
import { useActionStore } from '@/stores/useActionStore'

import FileUpload from '@/components/FileUpload.vue'
import { Files,Plus } from '@element-plus/icons-vue'

const fileApi = new FileApi(apiConfiguration)
const fileStore = useFileStore()
const actionStore = useActionStore()
const activeDto = ref<FileMetadataDto>({})

const activeRow = (row: any) => {
  activeDto.value = row as FileMetadataDto;
  drawerStatus.value = !drawerStatus.value
  // ElMessage.success('当前点击的行数据：'+ activeDto.value.fileName)
}

const drawerStatus = ref(false)

const tableColumns = computed(() => {
  if (!fileStore.fileList || fileStore.fileList.length === 0) {
    return []
  }
  // 提取第一个对象的 keys
  return Object.keys(fileStore.fileList[0])
  // return Object.keys(fileList.value[0]).filter((key) => !excludeKeys.includes(key))
})

// 3. 映射表：把英文 key 转换成中文表头（非必须，如果不配置则默认显示 key 名字）
const columnMap: Record<string, string> = {
  uid: '预览',
  fileName: '文件名',
  fileSize: '文件大小',
  fileHash: '文件哈希',
  uploadTime: '上传时间',
  fileType:'文件类型',
  tags: '标签',
}

const descriptionsMap: Record<string, string> = {
  uid: '文件ID',
  fileName: '文件名',
  fileSize: '文件大小',
  fileHash: '文件哈希',
  uploadTime: '上传时间',
  fileType:'文件类型',
  tags: '标签',
}
onBeforeMount(async () => {
 await fileStore.fetchAllFiles(fileApi)
 actionStore.onAdd = handleAddFile
 activeDto.value = {fileName:'',fileHash:'',fileType:'',fileSize:0}
})

function handleAddFile(){
  drawerStatus.value = !drawerStatus.value
  activeDto.value = {
    fileHash:'请上传后获取',
    fileName:'请上传后获取',
    tags:[],
    fileSize:0,
    fileType:'请上传后获取',
    uploadTime:'请上传后获取',
    uid:'请上传后获取'
  }
}


onBeforeUnmount(()=>actionStore.resetActions())
</script>
