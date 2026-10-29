<template>
  <ElImage
    v-if="isImageType(props.row.fileType!)"
    class="image"
    :src="fileStore.previewUrl.get(props.row.uid!)"
    @click.stop="emit('preview-image', props.row.uid!)"
  >
    <template #error>加载中...</template>
  </ElImage>
  <div v-else-if="isVideoType(props.row.fileType ?? '')" class="file-type-placeholder video-placeholder">
    <ElIcon><VideoCamera /></ElIcon>
  </div>
  <div v-else class="file-type-placeholder">
    <ElIcon>
      <component :is="mgsIconRegistry[MgsIconName.Markdown]" />
    </ElIcon>
  </div>
  <!-- <ElCol>{{ props.row.uid }}</ElCol> -->
</template>

<script setup lang="ts">
import { type FileDto } from '@/api'
import { VideoCamera } from '@element-plus/icons-vue'
import { MgsIconName, mgsIconRegistry } from '@aqlife/icons';
import { isImageType } from '@aqlife/domain'
import { useFileStore } from '@/stores/useFileStore';
const props = defineProps<{ row: FileDto }>()
const emit = defineEmits<{ 'preview-image': [uid: string] }>()

const fileStore = useFileStore()

const VIDEO_EXTENSIONS = new Set(['.mp4', '.webm'])

function isVideoType(fileType: string): boolean {
  const normalized = fileType.trim().toLowerCase()
  const extension = normalized.includes('.') ? normalized.slice(normalized.lastIndexOf('.')) : ''
  return VIDEO_EXTENSIONS.has(extension)
}
</script>

<style scoped>
.file-type-placeholder {
  width: 100%;
  height: 72px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--el-text-color-secondary);
  background: var(--el-fill-color-light);
  border-radius: 4px;
  font-size: 32px;
}

.video-placeholder {
  color: var(--el-color-primary);
}
</style>
