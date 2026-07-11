<template>
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
</template>

<script setup lang="ts">
import TagSelect from './TagSelect.vue'
import { MgsIconName,mgsIconRegistry } from '@aqlife/icons'
import { computed, onBeforeMount, reactive, ref, type Ref } from 'vue'
import { TagApi, type FileMetadataDto, type TagDto } from '@/api'
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
} from 'element-plus'
import { defaultFilePolicy } from '@aqlife/domain'
import { apiConfiguration } from '@/services/api'
import { useTagStore } from '@/stores/uuseTagStore'

const markdown = MgsIconName.Markdown

const acceptType = computed(() => {
  const fileTypes = defaultFilePolicy.allowedUpload.join(',')
  // console.log(fileTypes) // 输出: ".svg,.jpeg,.jpg,.png"
  return fileTypes
})

const imageExtensions = computed(() => {
  return defaultFilePolicy.allowedUpload.filter((item) => item !== '.md') // 过滤掉不需要的 .md，只留下图片
})

const isImageType = (f: UploadFile) => {
  if (!f.name) return false

  // 提取后缀名并转为小写
  const ext = f.name.substring(f.name.lastIndexOf('.')).toLowerCase()
  // 检查提取出的后缀是否存在于图片白名单中
  return imageExtensions.value.includes(ext)
}


// 依赖属性
const fileList = defineModel<UploadUserFile[]>('fileList', { default: () => [] })
const tags = defineModel<TagDto[]>('tags', { default: () => [] })

const selectDisabled = computed(() => fileList.value.length <= 0)
function remove(index:number){
  fileList.value.splice(index,1)
  ElMessage.success('移除文件成功')
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
