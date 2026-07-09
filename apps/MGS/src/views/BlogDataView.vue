<template>
  <ElTable :data="fileList">
    <ElTableColumn
      sortable
      :prop="item"
      v-for="item,index in tableColumns"
      :key="item"
      :label="columnMap[item] ?? item"
    >
      <template #default="scope" v-if="index==0">
        <AuthImage :uid="scope.row.uid" />
      </template>
    </ElTableColumn>
  </ElTable>
</template>

<script setup lang="ts">
import { ElTable, ElTableColumn, ElImage } from 'element-plus'
import { FileApi, type FileMetadataDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { onBeforeMount, ref, computed } from 'vue'
import AuthImage from '@/components/AuthImage.vue'
const fileApi = new FileApi(apiConfiguration)

const fileList = ref<FileMetadataDto[]>([])
// const excludeKeys = ['uid'] // 不需要展示的黑名单
const tableColumns = computed(() => {
  if (!fileList.value || fileList.value.length === 0) {
    return []
  }
  // 提取第一个对象的 keys
  return Object.keys(fileList.value[0])
  // return Object.keys(fileList.value[0]).filter((key) => !excludeKeys.includes(key))
})

const preview = (guid: string | null | undefined): string =>
  guid ? `${import.meta.env.VITE_API}/api/File/preview?UID=${guid}` : ''

async function previewHasBearerToken(guid: string) {
  return await fileApi.apiFilePreviewGet({ uID: guid })
}

// 3. 映射表：把英文 key 转换成中文表头（非必须，如果不配置则默认显示 key 名字）
const columnMap: Record<string, string> = {
  uid: '预览',
  fileName: '文件名',
  fileSize: '文件大小',
  fileHash: '文件哈希',
  uploadTime: '上传时间',
  tags: '标签',
}
onBeforeMount(async () => {
  fileList.value = await fileApi.apiFileGet()
  // console.log(fileList.value)
})
</script>
