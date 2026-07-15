<template>
  <ElDrawer v-model="drawerStatus" :title="fileDto?.fileName" with-header :show-close="false" @close="close">

    <ElDescriptions border :column="1">
      <ElDescriptionsItem label="预览">
        <ElImage v-if="isImageType(fileDto!.fileType!)" :src="useFileStore().previewUrl.get(fileDto!.uid!)"
          class="image" />
        <ElUpload v-else :disabled="isImageType(fileDto!.fileType!)" v-model:file-list="fileList"
          :accept="fileDto!.fileType" :limit="1" :auto-upload="false">
          <ElImage class="image" src="">
            <template #error>
              <ElIcon>
                <component :is="mgsIconRegistry[markdown]" />
              </ElIcon>
            </template>
          </ElImage>
          <template #tip> 仅允许{{ fileDto?.fileType }}类型文件上传 </template>
        </ElUpload>
      </ElDescriptionsItem>

      <ElDescriptionsItem label="标签">
        <TagSelect v-model:tag-list="selectedTags" />
      </ElDescriptionsItem>
    </ElDescriptions>
    <template #footer>
      <ElButton type="warning" @click="commit">Update</ElButton>
    </template>
  </ElDrawer>
</template>

<script setup lang="ts">
import { isImageType } from '@aqlife/domain'
import TagSelect from './TagSelect.vue'
import { MgsIconName, mgsIconRegistry } from '@aqlife/icons'
import { useTagStore } from '@/stores/uuseTagStore'
import { useFileStore } from '@/stores/useFileStore'
import { computed, reactive, ref } from 'vue'
import { FileApi, type FileMetadataDto, type TagDto } from '@/api'
import { UploadFilled, Files, Plus } from '@element-plus/icons-vue'
import { normalizeExtension } from '@aqlife/domain'
import ImageUpload from '@/components/ImageUpload.vue'
import {
  ElDescriptions,
  ElImage,
  ElButton,
  ElIcon,
  ElDescriptionsItem,
  type UploadUserFile,
  type UploadFile,
  ElMessage,
} from 'element-plus'
import { defaultFilePolicy } from '@aqlife/domain'
import { OperationalState, useActionStore } from '@/stores/useActionStore.ts'
import { apiConfiguration } from '@/services/api.ts'

const fileDto = defineModel<FileMetadataDto>()// 主要是为了获取文件类型来决定组件渲染方式
const props = defineProps<{
  initialTags?: TagDto[]
}>()

// 防止tag修改渗透,仅允许在update事件成功以后,由update回调至fileDto
const fileList = reactive<UploadUserFile[]>([])// 暂时忽略
const markdown = MgsIconName.Markdown
const actionStore = useActionStore()

const drawerStatus = computed(() => actionStore.OState == OperationalState.Update)
const selectedTags = ref<TagDto[]>(props.initialTags ? [...props.initialTags] : [])

function close() {
  actionStore.OState = OperationalState.None
}

async function commit() {
  if (!fileDto.value?.uid) {
    ElMessage.error('无法定位文件 UID')
    return
  }

  try {
    const fileApi = new FileApi(apiConfiguration)

    // A. 提取标签 UID 集合（只要是文件就可以更新） [cite: 16]
    const tagUids: string[] = selectedTags.value
      .map((tag) => tag.uid)
      .filter((uid): uid is string => !!uid)

    // B. 执行标签 Patch 关联
    const updatedGuid = await fileApi.apiFileTagPatch({
      updateFileTagCommand: { uid: fileDto.value.uid, tags: tagUids },
    })

    // C. 检查是否有新文件内容需要覆盖（解绑类型限制，有文件即更新）
    // if (fileList.length > 0) {
    //   const blobs = fileList
    //     .map((f) => f.raw)
    //     .filter((raw): raw is  => !!raw)

    //   if (blobs.length > 0) {
    //     // 执行文件流上传/更新契约 [cite: 11]
    //     await fileApi.apiFileUploadPost({ file: blobs })
    //   }
    // }

    // D. 状态同步：获取最新 DTO 并刷新全局 Store [cite: 9]
    const latestDtos = await fileApi.apiFileGet({ uID: updatedGuid })
    if (latestDtos && latestDtos.length > 0) {
      const updatedDto = latestDtos[0]

      // 找到 Store 中的旧对象并替换，确保表格即时刷新 [cite: 11]
      const store = useFileStore()
      const index = store.fileList.findIndex(f => f.uid === updatedGuid)
      if (index !== -1) {
        store.fileList[index] = updatedDto
      }

      // 同步给本地 Model 并关闭弹窗
      fileDto.value = updatedDto
      ElMessage.success('文件 Tag 已同步更新')
      close()
    }
  } catch (error) {
    // 捕获并处理业务异常，如数据库连接异常或请求事务失败 [cite: 8, 10]
    ElMessage.error('更新失败，请检查文件状态或网络连接')
    console.error('Commit Error:', error)
  }
}
</script>

<style lang="css" scoped>
.fillIcon {
  font-size: 5rem;
}

.image {
  height: 10vw;
  width: 10vw;
  font-size: 10vw;
}

.maxText {
  width: clac(100% - 20px);
  overflow: hidden;
  padding: 20px;
}
</style>
