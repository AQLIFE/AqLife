<template>
  <ElDrawer v-model="drawerStatus" title="上传新文件" with-header :show-close="false" @close="close">
  <ElDescriptions border :column="1">
    <ElDescriptionsItem label="上传">
      <ElUpload
        v-model:file-list="fileList"
        :accept="acceptType"
        drag
        multiple
        list-type="picture"
        show-file-list
        :auto-upload="false"
      >
        <ElIcon class="fillIcon"><UploadFilled /></ElIcon>
        <div class="el-upload__text">Drop file here or <em>click to upload</em></div>
        <template #tip> 仅允许{{ acceptType }}类型文件上传 </template>
        <template #file="{ file, index }">
          <!-- Elemnt-plus 在 TS 开发下,插槽参数被降级为 Any,非常狗血 -->
          <ElImage class="image" :src="isImageType(file) ? file.url : ''" :key="index">
            <template #error>
              <ElIcon><component :is="mgsIconRegistry[markdown]" /></ElIcon>
            </template>
          </ElImage>
          <ElText class="maxText">{{ file.name }}</ElText>
          <ElButton :icon="Delete" @click="remove(index)"></ElButton>
        </template>
      </ElUpload>
    </ElDescriptionsItem>
    <ElDescriptionsItem label="标签">
      <TagSelect :select-disabled="selectDisabled" v-model:tag-list="tags"/>
    </ElDescriptionsItem>
  </ElDescriptions>
  <template #footer>
      <ElButton type="warning" @click="commit">commit</ElButton>
    </template>
  </ElDrawer>
</template>


<script setup lang="ts">
import { isImageType } from '@aqlife/domain'
import { useActionStore } from '@/stores/useActionStore.ts'
import TagSelect from './TagSelect.vue'
import { MgsIconName,mgsIconRegistry } from '@aqlife/icons'
import { computed, onBeforeMount, reactive, ref, type Ref } from 'vue'
import { FileApi, TagApi, type FileMetadataDto, type TagDto } from '@/api'
import { UploadFilled, Files, Plus, Delete } from '@element-plus/icons-vue'

import {
  ElDescriptions,
  ElImage,
  ElOption,
  ElButton,
  ElInput,
  ElIcon,
  ElSelect,
  ElOptionGroup,
  ElDescriptionsItem,
  type UploadUserFile,
  type UploadFile,
  ElMessage,
  ElLoading,
  type UploadRawFile,
} from 'element-plus'
import { defaultFilePolicy } from '@aqlife/domain'
import { apiConfiguration } from '@/services/api'
import { useTagStore } from '@/stores/uuseTagStore'
import { OperationalState } from '@/stores/useActionStore.ts'
import { useFileStore } from '@/stores/useFileStore.ts'

///---- 组件属性
const markdown = MgsIconName.Markdown // md 文件 logo
const acceptType = computed(()=>defaultFilePolicy.allowedUpload.join(','))// 允许上传所有支持的类型


// 依赖属性
const actionStore = useActionStore()
const fileStore = useFileStore()
const fileApi = new FileApi(apiConfiguration)

const fileList = defineModel<UploadUserFile[]>('fileList', { default: () => [] })
const tags = defineModel<TagDto[]>('tags', { default: () => [] })

const drawerStatus = computed(()=>actionStore.OState == OperationalState.Add)
const selectDisabled = computed(() => fileList.value.length <= 0)

// 组件方法
function remove(index:number){
  fileList.value.splice(index,1)
  ElMessage.success('移除文件成功')
}

function close(){
  actionStore.OState = OperationalState.None
  console.log(actionStore.OState)
}

async function commit() {
  // 1. 准入校验：非添加状态或无文件则直接返回 (Fail-Fast) [cite: 16]
  if ( fileList.value.length === 0) return

  const loading = ElLoading.service({ text: '正在处理上传任务...' })
  try {
    // 开启全局 Loading 提示 (可选)

    // 2. 转换并上传原始文件流 [cite: 11]
    const blobs: Blob[] = fileList.value
      .map((file) => file.raw)
      .filter((raw): raw is UploadRawFile => !!raw)

    const tempGuidGroup = await fileApi.apiFileUploadPost({ file: blobs })

    // 3. 准备标签数据
    const innerTags: string[] = tags.value
      .map((tag) => tag.uid)
      .filter((uid): uid is string => uid != null)
    // console.log(tags, tags)

    // 4. 并行处理关联逻辑 (核心优化点) [cite: 9]
    // 即使 tags 为空，我们也要获取文件元数据以更新 UI [cite: 10]
    const updateTasks = tempGuidGroup.map(async (uid) => {
      try {
        let finalUid = uid

        // 只有存在标签时才调用 Patch 接口
        if (innerTags.length > 0) {
          finalUid = await fileApi.apiFileTagPatch({
            updateFileTagCommand: { uid, tags:innerTags },
          })
        }

        // 获取最新的文件 DTO (包含元数据和 已关联的标签) [cite: 9]
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
      fileList.value = []
      tags.value = []
    }
  } catch (error) {
    ElMessage.error('上传流程发生异常，请检查网络或后端日志')
    console.error('Commit Error:', error)
  }
  loading.close()
  // drawerStatus.value=!drawerStatus.value
  actionStore.OState = OperationalState.None //drawerStatus 被影响此时会关闭
}
</script>

<style lang="css" scoped>
.fillIcon {
  font-size: 5rem;
}
.image {
  max-height: 6vw;
  max-width: 6vw;
  min-width: 6vw;
  min-height: 6vw;
}

.image :deep(.el-icon) {
  font-size: 5vw;
  position: relative;
  top: 0.5vw;
}
.maxText {
  max-width: calc(100% - 10vw);
  overflow: hidden;
  padding: 20px;
}
</style>
