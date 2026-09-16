<template>
  <ElDrawer v-model="drawerStatus" :title="fileDto?.fileName" with-header :show-close="false">
    <ElDescriptions border :column="1" label-width="120px">
      <ElDescriptionsItem label="ID" align="center">
        {{ fileDto?.uid }}
      </ElDescriptionsItem>

      <ElDescriptionsItem label="预览" align="center">
        <ElImage v-if="isImageType(fileDto!.fileType!)" :src="useFileStore().previewUrl.get(fileDto!.uid!)"
          class="image" />
        <ImageUpload v-else v-model:file="PrivateFileQueue" :accept="fileDto!.fileType!">
          <component :is="mgsIconRegistry[MgsIconName.Markdown]"/>
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
        <TagSelect v-model:tag-list="selectedTags" />
      </ElDescriptionsItem>

      <ElDescriptionsItem label="发布状态">
        <ElSegmented v-model="fileDto!.publishStatus" :options="demo">
          <template #default="scope">
            <ElIcon><component :is="scope.item.icon"/></ElIcon>
            <div>{{ scope.item.label }}</div>
          </template>
        </ElSegmented>
      </ElDescriptionsItem>

      <ElDescriptionsItem label="预定发布时间" v-if="fileDto?.publishStatus!=publishStatus.Draft">
        <ElDatePicker :disabled="fileDto?.publishStatus==publishStatus.Published" v-model="fileDto!.publishAt"/>
      </ElDescriptionsItem>

      <ElDescriptionsItem label="文件类型">
        {{ fileDto?.fileType }}
      </ElDescriptionsItem>
    </ElDescriptions>
    <template #footer>
      <ElButton type="success" @click="preview" v-if="!isImageType(fileDto!.fileType!)">Preview</ElButton>
      <ElButton type="warning" @click="update" :disabled="!canUpdateFile">Update</ElButton>
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
} from 'element-plus'
import { OperationalState, useActionStore } from '@/stores/useActionStore.ts'
import { apiConfiguration } from '@/services/api.ts'
import { useRouter } from 'vue-router'
import { FileTagUpdateWorkflow } from '@/workflow/fileflow/FileTagUpdateWorkFlow.ts'
import { FileContentUpdateWrkflow } from '@/workflow/fileflow/FileContentUpdateWorkflow.ts'
import { Brush, Promotion, Timer } from '@element-plus/icons-vue'

enum publishStatus {Draft,Scheduled,Published}
const demo:{label:string,value:publishStatus,icon:Component}[] = [
  {label:'草稿',icon:Brush,value:publishStatus.Draft},
  {label:'预约',icon:Timer,value:publishStatus.Scheduled},
  {label:'发布',icon:Promotion,value:publishStatus.Published}
]
// const test = ref(publishStatus.Scheduled)

const actionStore = useActionStore()
const router = useRouter()

const fileDto = defineModel<FileDto>()// 主要是为了获取文件类型来决定组件渲染方式

// 防止tag修改渗透,仅允许在update事件成功以后,由update回调至fileDto
const PrivateFileQueue = ref<File | null>(null)// 暂时忽略
const fileValidationMessage = computed(() => {
  const file = PrivateFileQueue.value
  const original = fileDto.value

  if (!file || !original) {
    return ''
  }

  if (file.name !== original.fileName) {
    return `只能上传与原文件同名的文件：${original.fileName};你上传的文件叫做${PrivateFileQueue.value?.name}`
  }

  return ''
})
const canUpdateFile = computed(() => {
  return PrivateFileQueue.value !== null &&
    fileValidationMessage.value === ''
})

const drawerStatus = computed({
  get: () => actionStore.OState === OperationalState.Update,

  set: (value: boolean) => {
    if (!value) {
      actionStore.OState = OperationalState.None
    }
  }
})
const selectedTags: Ref<TagDto[]> = ref<TagDto[]>([])
watch(
  () => fileDto.value?.uid,
  () => {
    selectedTags.value = fileDto.value?.tags ?? []
  },
  { immediate: true }
)

async function update() {
  if(!PrivateFileQueue.value || !fileDto.value)return
  try {
    const fileApi = new FileApi(apiConfiguration)
    const updateFileTag = new FileTagUpdateWorkflow(fileDto.value,fileApi,useFileStore())
    const updateFile = new FileContentUpdateWrkflow(fileDto.value,PrivateFileQueue.value,fileApi,useFileStore())
    updateFile.run()
    updateFileTag.run()

  } catch (error) {
    ElMessage.error('更新失败，请检查文件状态或网络连接')
    console.error('Commit Error:', error)
  }
}

function preview() {
  actionStore.OState = OperationalState.View
  actionStore.cacheViewGuid = fileDto.value?.uid ?? ''
  router.push('/blog/view')
}

watch(
  PrivateFileQueue,
  (file) => {
    if (!file || !fileDto.value) return

    if (file.name !== fileDto.value.fileName) {
      ElMessage.warning(
        `文件名不匹配，只允许上传：${fileDto.value.fileName}`
      )
    }
  }
)
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
