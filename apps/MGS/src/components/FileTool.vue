<template>
  <ElDrawer v-model="drawerStatus" :title="fileDto?.fileName" with-header :show-close="false">
    <ElDescriptions border :column="1" label-width="120px">
      <ElDescriptionsItem label="ID" align="center">
        {{ fileDto?.uid }}
      </ElDescriptionsItem>

      <ElDescriptionsItem label="预览" align="center">
        <ElImage v-if="isImageType(fileDto!.fileType!)" :src="useFileStore().previewUrl.get(fileDto!.uid!)"
          class="image" />
        <ImageUpload v-else v-model:file="pendingFile" :accept="fileDto!.fileType!" @update:file="onFileChanged">
          <component :is="mgsIconRegistry[MgsIconName.Markdown]" />
          <template #tip>
            <span :class="{ 'upload-tip-error': fileValidationMessage }">
              {{
                fileValidationMessage ||
                '可上传同名的文件用于进行更新'
              }}
            </span>
          </template>
        </ImageUpload>
      </ElDescriptionsItem>

      <ElDescriptionsItem label="标签">
        <TagSelect v-model:tag-list="selectedTags" @update:tag-list="onTagsChanged" />
      </ElDescriptionsItem>

      <ElDescriptionsItem label="发布状态">
        <ElSegmented :model-value="fileDto!.publishStatus" :options="segmentedOptions"
          @update:model-value="onPublishStatusChanged" :disabled="publishing">
          <template #default="scope">
            <ElIcon>
              <component :is="scope.item.icon" />
            </ElIcon>
            <div>{{ scope.item.label }}</div>
          </template>
        </ElSegmented>
      </ElDescriptionsItem>

      <ElDescriptionsItem label="预定发布时间" v-if="fileDto?.publishStatus != publishStatus.Draft">
        <ElDatePicker :disabled="fileDto?.publishStatus == publishStatus.Published" v-model="fileDto!.publishAt"
          @blur="onDateTimeChanged" />
      </ElDescriptionsItem>

      <ElDescriptionsItem label="文件类型">
        {{ fileDto?.fileType }}
      </ElDescriptionsItem>
    </ElDescriptions>

    <template #footer>
      <ElButton type="success" @click="preview" v-if="!isImageType(fileDto!.fileType!)">Preview</ElButton>
      <ElButton type="danger" @click="deleteFile" title="物理删除" :loading="deleting">Delete</ElButton>
    </template>
  </ElDrawer>
</template>

<script setup lang="ts">
import { isImageType } from '@aqlife/domain'
import TagSelect from './TagSelect.vue'
import { MgsIconName, mgsIconRegistry } from '@aqlife/icons'
import { useFileStore } from '@/stores/useFileStore'
import { computed, ref, watch, type Component, type Ref } from 'vue'
import { FileApi, type FileDto, type TagDto } from '@/api'
import ImageUpload from '@/components/ImageUpload.vue'
import {
  ElSegmented,
  ElDatePicker,
  ElDescriptions,
  ElImage,
  ElButton,
  ElDescriptionsItem,
  ElMessage,
  ElMessageBox,
} from 'element-plus'
import { OperationalState, useActionStore } from '@/stores/useActionStore.ts'
import { apiConfiguration } from '@/services/api.ts'
import { useRouter } from 'vue-router'
import { Brush, Promotion, Timer } from '@element-plus/icons-vue'
import { FileDraftWorkflow,FileContentUpdateWorkflow,FilePublishWorkflow,FileDeleteWorkflow,FileTagUpdateWorkflow,FileScheduledWorkflow } from '@/workflow/index.ts'
//src/workflow/index.ts:4:15 - error TS1261: Already included file name 'F:/Code/Mylife/AqLife/apps/MGS/src/workflow/fileflow/FilescheduledWorkflow.ts' differs from file name 'F:/Code/Mylife/AqLife/apps/MGS/src/workflow/fileflow/FileScheduledWorkflow.ts' only in casing.
//   The file is in the program because:
//     Imported via './fileflow/FilescheduledWorkflow' from file 'F:/Code/Mylife/AqLife/apps/MGS/src/workflow/index.ts'
//     Matched by include pattern 'src/**/*' in 'F:/Code/Mylife/AqLife/apps/MGS/tsconfig.app.json'

// 4 export * from './fileflow/FilescheduledWorkflow'
//                 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

//   tsconfig.app.json:3:27
//     3   "include": ["env.d.ts", "src/**/*", "src/**/*.vue"],
//                                 ~~~~~~~~~~
//     File is matched by include pattern specified here.


const fileApi = new FileApi(apiConfiguration)
const fileStore = useFileStore()
const actionStore = useActionStore()
const router = useRouter()


//================================组件视觉属性
function preview() {
  actionStore.OState = OperationalState.View
  actionStore.cacheViewGuid = fileDto.value?.uid ?? ''
  router.push('/blog/view')
}
const drawerStatus = computed({
  get: () => actionStore.OState === OperationalState.Update,

  set: (value: boolean) => {
    if (!value) {
      actionStore.OState = OperationalState.None
    }
  }
})

//================================fileDto
const fileDto = defineModel<FileDto>()

// 防止tag修改渗透,仅允许在update事件成功以后,由update回调至fileDto
const pendingFile = ref<File | null>(null)// 暂时忽略

//================================File 删除
const deleting = ref(false)
async function deleteFile() {
  if (!fileDto.value?.uid) return

  try {
    await ElMessageBox.confirm(
      `确定删除「${fileDto.value.fileName}」吗？`,
      '删除文件',
      {
        type: 'warning',
        confirmButtonText: '删除',
        cancelButtonText: '取消'
      }
    )

    deleting.value = true

    const workflow = new FileDeleteWorkflow(
      fileDto.value,
      fileApi,
      fileStore
    )

    await workflow.run()

    actionStore.OState = OperationalState.None
  } catch (error) {
    // 用户取消不提示错误
    if (error !== 'cancel') {
      ElMessage.error('删除失败，请检查文件状态或网络连接')
      console.error('Delete Error:', error)
    }
  } finally {
    deleting.value = false
  }
}

//================================tag 更新
const selectedTags: Ref<TagDto[]> = ref<TagDto[]>([])
const tagUpdating = ref(false)
async function onTagsChanged(value: TagDto[] | undefined) {
  if (!fileDto.value?.uid) return

  try {
    tagUpdating.value = true
    if (!value) throw new Error("无效的tag")
    const workflow = new FileTagUpdateWorkflow(
      fileDto.value,
      value,
      fileApi,
      fileStore
    )

    const dto = await workflow.run()

    fileDto.value = dto
    ElMessage.success(`${fileDto.value.fileName} 更新 tag 完成`)
  } catch {
    ElMessage.error('标签更新失败')

    // 回滚 selectedTags
    selectedTags.value = fileDto.value?.tags ?? []
  } finally {
    tagUpdating.value = false
  }
}
//================================发布状态
enum publishStatus { Draft, Scheduled, Published }
const demo: { label: string, value: publishStatus, icon: Component }[] = [
  { label: '草稿', icon: Brush, value: publishStatus.Draft },
  { label: '预约', icon: Timer, value: publishStatus.Scheduled },
  { label: '发布', icon: Promotion, value: publishStatus.Published }
]
const segmentedOptions = computed(() =>
  demo.map(item => ({
    ...item,
    disabled:
      item.value === publishStatus.Scheduled &&
      fileDto.value?.publishStatus === publishStatus.Published,
  }))
)
const publishStatusModel = ref<publishStatus>()
watch(
  () => fileDto.value?.publishStatus,
  value => {
    publishStatusModel.value = value
  },
  { immediate: true }
)
const publishing = ref(false)
async function onPublishStatusChanged(value: publishStatus) {
  const file = fileDto.value

  if (!file?.uid) return

  const currentStatus = file.publishStatus

  if (currentStatus === value) return

  try {
    publishing.value = true

    let dto: FileDto

    switch (value) {
      case publishStatus.Draft:
        dto = await changeToDraft(file)
        break

      case publishStatus.Scheduled:
        dto = await changeToScheduled(file)
        break

      case publishStatus.Published:
        dto = await changeToPublished(file)
        break

      default:
        return
    }

    fileDto.value = dto
  } catch {
    ElMessage.error('文件状态更新失败')
  } finally {
    publishing.value = false
  }
}
// 立即发布
async function changeToPublished(file: FileDto) {
  const workflow = new FilePublishWorkflow(
    file, new FileApi(apiConfiguration), useFileStore()
  )
  const dto =await workflow.run()
  ElMessage.success(`${file.fileName} 发布成功`)
  return dto
}

async function changeToDraft(value: FileDto) {
  const workflow = new FileDraftWorkflow(
    value, new FileApi(apiConfiguration), useFileStore()
  )
  const dto = await workflow.run()
  ElMessage.success(`${value.fileName} 设置为草稿`)
  return dto
}

const scheduling = ref(false)
async function onDateTimeChanged() {

  if (!fileDto.value?.publishAt) {
    return
  }

  const file = fileDto.value

  try {
    publishing.value = true

    const workflow = new FileScheduledWorkflow(
      file,
      fileApi,
      fileStore
    )

    const dto = await workflow.run()

    fileDto.value = dto
    scheduling.value = false

    ElMessage.success(`${file.fileName} 设置预定发布完成`)
  } catch {
    ElMessage.error('设置预定发布时间失败')
  } finally {
    publishing.value = false
  }
}

// 需要等待时间选择器选择完成后才正式执行
async function changeToScheduled(value: FileDto) {
  if (value.publishStatus == publishStatus.Published) {
    ElMessage.error('禁止从已发布状态修改为预定发布状态')
    throw new Error()
  } else if (!value.publishAt) {
    ElMessage.warning('请设置预定时间')
    // throw new Error()
    value.publishStatus = publishStatus.Scheduled
    scheduling.value = true
  }
  return value
  // const workflow = new FileScheduledWorkflow(value,fileApi,fileStore)
  // const result = await workflow.run()
  // ElMessage.success(`${value.fileName} 设置预定发布完成`)
  // return result
}
//================================属性监听
async function onFileChanged(value: File | undefined | null) {
  console.log(value?.name,value?.size)
  if (!value || !fileDto.value) return

  if (value.name !== (fileDto.value.fileName!+fileDto.value.fileType)) {
    ElMessage.warning(
      `文件名不匹配，只允许上传：${fileDto.value.fileName}`
    )
    throw new Error()
  }

  const workflow = new FileContentUpdateWorkflow(fileDto.value,value,fileApi,fileStore)
  const dto =await workflow.run()
  ElMessage.success(`${fileDto.value.fileName} 更新完成`)
  return dto;
}
watch(
  () => fileDto.value?.uid,
  () => {
    selectedTags.value = fileDto.value?.tags ?? []
  },
  { immediate: true }
)
const fileValidationMessage = computed(() => {
  // const file = pendingFile.value
  const original = fileDto.value

  if (!pendingFile.value || !original) {
    return ''
  }

  if (pendingFile.value.name !== (original.fileName!+fileDto.value!.fileType)) {
    return `只能上传与原文件同名的文件：${original.fileName};你上传的文件叫做${pendingFile.value?.name}`
  }

  return ''
})
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

.upload-tip-error {
  color: var(--el-color-danger);
}
</style>
