<template>
  <ElDescriptions border :column="1">
    <ElDescriptionsItem label="预览">
      <ElImage v-if="isImageType(fileDto.fileType)"  :src="useFileStore().previewUrl.get(fileDto.uid!)" class="image"/>
      <ElUpload
        v-else
        :disabled="isImageType(fileDto.fileType)"
        v-model:file-list="fileList"
        :accept="fileDto.fileType"
        :limit="1"
        :auto-upload="false"
      >
        <ElImage class="image" src="">
          <template #error>
            <ElIcon><component :is="mgsIconRegistry[markdown]"/></ElIcon>
          </template>
        </ElImage>
        <template #tip> 仅允许{{ fileDto.fileType }}类型文件上传 </template>
      </ElUpload>
    </ElDescriptionsItem>

    <ElDescriptionsItem label="标签">
      <TagSelect v-model:tag-list="tags"/>
    </ElDescriptionsItem>
  </ElDescriptions>
</template>

<script setup lang="ts">
import TagSelect from './TagSelect.vue'
import { MgsIconName,mgsIconRegistry } from '@aqlife/icons'
import { useTagStore } from '@/stores/uuseTagStore'
import { useFileStore } from '@/stores/useFileStore'
import { computed, reactive } from 'vue'
import type { FileMetadataDto, TagDto } from '@/api'
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
} from 'element-plus'
import { defaultFilePolicy } from '@aqlife/domain'
const props = defineProps<{ fileDto: FileMetadataDto }>()// 主要是为了获取文件类型来决定组件渲染方式
const tags = defineModel<TagDto[]>('tags', { default: () => [] })
const fileList = reactive<UploadUserFile[]>([])// 仅在md 文件格式时才允许存在和更新
const markdown = MgsIconName.Markdown

const imageExtensions = computed(() => {
  return defaultFilePolicy.allowedUpload.filter((item) => item !== '.md') // 过滤掉不需要的 .md，只留下图片
})

const isImageType = (ext: string | undefined | null) => {
  // const f = file  // 顺手顺顺类型
  if (ext == '' || !ext) return false
  return imageExtensions.value.includes(ext)
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
