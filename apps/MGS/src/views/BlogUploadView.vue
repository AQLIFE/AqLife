<script setup lang="ts">
import { computed, ref } from 'vue'
import { ElButton, ElCard, ElIcon, ElMessage, ElUpload, type UploadUserFile, type UploadRawFile } from 'element-plus'
import { ArrowLeft, UploadFilled } from '@element-plus/icons-vue'
import { defaultFilePolicy } from '@aqlife/domain'

import { FileApi, type FileDto, type TagDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useFileStore } from '@/stores/useFileStore'
import TagSelect from '@/components/TagSelect.vue'
import MgsPageHeader from '@/components/ui/MgsPageHeader.vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const fileApi = new FileApi(apiConfiguration)
const fileStore = useFileStore()

const fileList = ref<UploadUserFile[]>([])
const tags = ref<TagDto[]>([])
const uploading = ref(false)

const acceptType = computed(() => defaultFilePolicy.allowedUpload.join(','))

const canCommit = computed(() => fileList.value.length > 0 && !uploading.value)

function goBack() {
  router.push('/blog/list')
}

async function commit() {
  if (!canCommit.value) return

  const blobs: Blob[] = fileList.value
    .map(file => file.raw)
    .filter((raw): raw is UploadRawFile => !!raw)

  if (blobs.length === 0) {
    ElMessage.warning('没有可上传的文件')
    return
  }

  uploading.value = true

  try {
    const tempGuidGroup = await fileApi.apiFileUploadPost({ file: blobs })

    const innerTags = tags.value
      .map(tag => tag.uid)
      .filter((uid): uid is string => uid != null)

    const updateTasks = tempGuidGroup.map(async uid => {
      let finalUid = uid

      if (innerTags.length > 0) {
        finalUid = await fileApi.apiFileTagPatch({
          updateFileTagCommand: { uid, tags: innerTags },
        })
      }

      const fileDtos = await fileApi.apiFileGet({ uID: finalUid })
      return fileDtos.items ?? []
    })

    const results = await Promise.all(updateTasks)
    const uploadedFiles = results.flat()

    uploadedFiles.forEach((dto: FileDto) => fileStore.upsertFile(dto))

    if (uploadedFiles.length > 0) {
      ElMessage.success(`成功上传 ${uploadedFiles.length} 个文件`)
    }

    fileList.value = []
    tags.value = []
    router.push('/blog/list')
  } catch (error) {
    console.error('Blog upload error:', error)
    ElMessage.error(error instanceof Error ? error.message : '上传流程发生异常，请检查网络或后端日志')
  } finally {
    uploading.value = false
  }
}
</script>

<template>
  <div class="upload-workspace" v-loading="uploading">
    <MgsPageHeader
      title="Upload"
      description="上传文章文件，并在上传前统一设置标签"
      :back="true"
    >
      <template #actions>
        <ElButton :icon="ArrowLeft" @click="goBack">
          返回文章列表
        </ElButton>
        <ElButton
          type="primary"
          :icon="UploadFilled"
          :disabled="!canCommit"
          @click="commit"
        >
          开始上传
        </ElButton>
      </template>
    </MgsPageHeader>

    <div class="upload-content">
      <ElCard shadow="never" class="upload-card">
        <template #header>
          <div class="section-title">文件</div>
        </template>

        <ElUpload
          v-model:file-list="fileList"
          :accept="acceptType"
          drag
          multiple
          show-file-list
          :auto-upload="false"
        >
          <ElIcon class="upload-icon"><UploadFilled /></ElIcon>
          <div class="el-upload__text">
            将文件拖到这里，或 <em>点击选择文件</em>
          </div>
          <template #tip>
            <div class="upload-tip">允许上传：{{ acceptType }}</div>
          </template>
        </ElUpload>
      </ElCard>

      <ElCard shadow="never" class="tag-card">
        <template #header>
          <div class="section-title">标签</div>
        </template>

        <TagSelect
          v-model:tag-list="tags"
          :select-disabled="fileList.length === 0"
        />

        <p class="helper-text">
          标签会应用到本次上传的所有文件。
        </p>
      </ElCard>
    </div>
  </div>
</template>

<style scoped>
.upload-workspace {
  min-height: 100%;
  display: flex;
  flex-direction: column;
}

.upload-content {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 320px;
  gap: 16px;
  padding-bottom: 24px;
}

.upload-card,
.tag-card {
  min-width: 0;
}

.section-title {
  font-weight: 600;
}

.upload-icon {
  font-size: 52px;
  margin-bottom: 10px;
}

.upload-tip,
.helper-text {
  color: var(--mgs-muted);
  font-size: 13px;
}

@media (max-width: 900px) {
  .upload-content {
    grid-template-columns: 1fr;
  }
}
</style>
