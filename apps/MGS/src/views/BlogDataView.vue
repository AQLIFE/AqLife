<template>
  <ElTable :data="fileStore.fileList" highlight-current-row @row-click="activeRow">
    <ElTableColumn
      sortable
      :prop="item"
      v-for="(item, index) in tableColumns"
      :key="item"
      :label="columnMap[item] ?? item"
    >
      <template #default="scope" v-if="index == 0">
        <ElImage
          :src="fileStore.previewUrl.get(scope.row.uid)"
          v-if="isImageType(scope.row.fileType)"
        >
          <template #error>加载中...</template>
        </ElImage>
        <ElImage src="" v-else class="image">
          <template #error>
            <ElIcon><component :is="mgsIconRegistry[markdown]" /></ElIcon>
          </template>
        </ElImage>
      </template>
      <template #default="scope" v-else-if="index == 2">
        <!-- <ElTag >scope.row.tags</ElTag> -->
        <template v-if="scope.row.tags.length > 0 && scope.row.tags != undefined">
          <ElTag v-for="(tag, tagKey) in scope.row.tags" :key="tagKey" class="gap">{{ tag.name }}</ElTag>
        </template>
        <ElTag
          ><ElIcon><Plus /></ElIcon
        ></ElTag>
      </template>
    </ElTableColumn>
  </ElTable>
  <ElDrawer
    v-model="drawerStatus"
    :title="actionStore.OState == OperationalState.Add ? '上传文件' : '文件处理器'"
    with-header
    :show-close="false"
  >
    <FileUpload
      v-if="actionStore.OState == OperationalState.Add"
      v-model:file-list="UploadContext.fileList"
      v-model:tags="UploadContext.tags"
    />
    <FileTool v-else :file-dto="activeDto"  v-model:tags="activeDto.tags!" />
    <template #footer>
      <ElButton type="warning" @click="commit">commit</ElButton>
    </template>
  </ElDrawer>
</template>

<script setup lang="ts">
import { MgsIconName, mgsIconRegistry } from '@aqlife/icons'
const markdown = MgsIconName.Markdown
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
// const fileList = ref<UploadUserFile[]>([])
const UploadContext = reactive<{ fileList: UploadUserFile[]; tags: TagDto[] }>({
  fileList: [],
  tags: [],
})
// ---
const fileApi = new FileApi(apiConfiguration)
const fileStore = useFileStore()
const actionStore = useActionStore()
const activeDto = ref<FileMetadataDto>({})

const activeRow = (row: any) => {
  actionStore.OState = OperationalState.None
  activeDto.value = row as FileMetadataDto
  drawerStatus.value = !drawerStatus.value
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
  drawerStatus.value = !drawerStatus.value
}

const imageExtensions = computed(() => {
  return defaultFilePolicy.allowedUpload.filter((item) => item !== '.md') // 过滤掉不需要的 .md，只留下图片
})

const isImageType = (ext: string) => {
  // if (!f.name) return false

  // 提取后缀名并转为小写
  // const ext = f.name.substring(f.name.lastIndexOf('.')).toLowerCase()
  // 检查提取出的后缀是否存在于图片白名单中
  return imageExtensions.value.includes(ext)
}

async function commit() {
  // 1. 准入校验：非添加状态或无文件则直接返回 (Fail-Fast) [cite: 16]
  if (actionStore.OState !== OperationalState.Add || UploadContext.fileList.length === 0) return

  const loading = ElLoading.service({ text: '正在处理上传任务...' })
  try {
    // 开启全局 Loading 提示 (可选)

    // 2. 转换并上传原始文件流 [cite: 11]
    const blobs: Blob[] = UploadContext.fileList
      .map((file) => file.raw)
      .filter((raw): raw is UploadRawFile => !!raw)

    const tempGuidGroup = await fileApi.apiFileUploadPost({ file: blobs })

    // 3. 准备标签数据
    const tags: string[] = UploadContext.tags
      .map((tag) => tag.uid)
      .filter((uid): uid is string => uid != null)
    console.log(tags, UploadContext.tags)

    // 4. 并行处理关联逻辑 (核心优化点) [cite: 9]
    // 即使 tags 为空，我们也要获取文件元数据以更新 UI [cite: 10]
    const updateTasks = tempGuidGroup.map(async (uid) => {
      try {
        let finalUid = uid

        // 只有存在标签时才调用 Patch 接口
        if (tags.length > 0) {
          finalUid = await fileApi.apiFileTagPatch({
            updateFileTagCommand: { uid, tags },
          })
        }

        // 获取最新的文件 DTO (包含元数据和已关联的标签) [cite: 9]
        const fileDtos = await fileApi.apiFileGet({ uID: finalUid })
        // ElMessage.info(fileDtos[0].fileName??'失败')
        return fileDtos
      } catch (err) {
        console.error(`处理文件[${uid}]失败:`, err)
        return null
      }
    })

    // 等待所有并行任务完成
    const results = await Promise.all(updateTasks)

    // 5. 批量更新 Store，减少 Vue 响应式触发次数 [cite: 11]
    const validDtos = results.filter((dto): dto is FileMetadataDto[] => !!dto)
    if (validDtos.length > 0) {
      fileStore.fileList.push(...validDtos[0])
      ElMessage.success(`成功处理 ${validDtos.length} 个文件`)
      // 重置上下文
      UploadContext.fileList = []
      UploadContext.tags = []
    }
  } catch (error) {
    ElMessage.error('上传流程发生异常，请检查网络或后端日志')
    console.error('Commit Error:', error)
  }
  loading.close()
  drawerStatus.value=!drawerStatus.value
}

onBeforeUnmount(() => actionStore.resetActions())
</script>

<style lang="css" scoped>
.image {
  max-height: 10vw;
  max-width: 10vw;
  min-width: 10vw;
  min-height: 10vw;
  font-size: 10vw;
}
.gap{margin-right: 10px;}
</style>
