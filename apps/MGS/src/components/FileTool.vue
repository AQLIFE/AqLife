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
          <ElImage class="image" :src="isImageType(file) ? file.url : ''" :key="index" style="font-size: 3rem">
            <template #error>
              <ElIcon><Files /></ElIcon>
            </template>
          </ElImage>
          <ElText class="maxText">{{ file.name }}</ElText>
        </template>
      </ElUpload>
    </ElDescriptionsItem>

    <ElDescriptionsItem
      v-for="(item, index) in Object.keys(fileDto)"
      :key="index"
      :label="columnMap[item]"
    >
    <template v-if="index==2">
      <ElTag v-for="tag,tagKey in Object.values(fileDto)[index]" :key="tagKey">{{ tag }}</ElTag>
      <ElTag ><ElIcon><Plus/></ElIcon></ElTag>
    </template>
    <ElText v-else>{{ Object.values(fileDto)[index] }}</ElText>
    </ElDescriptionsItem>
  </ElDescriptions>
</template>

<script setup lang="ts">
import { computed, reactive } from 'vue'
import type { FileMetadataDto } from '@/api'
import { UploadFilled, Files,Plus } from '@element-plus/icons-vue'
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
const props = defineProps<{ fileDto: FileMetadataDto; columnMap: Record<string, string> }>()
const fileList = reactive<UploadUserFile[]>([])
const acceptType = computed(() => {
  const fileTypes = defaultFilePolicy.allowedUpload.join(',')
  // console.log(fileTypes) // 输出: ".svg,.jpeg,.jpg,.png"
  return fileTypes
})

const imageExtensions = computed(() => {
  return defaultFilePolicy.allowedUpload.filter((item) => item !== '.md') // 过滤掉不需要的 .md，只留下图片
})

const isImageType = (f: UploadFile) => {
  // const f = file  // 顺手顺顺类型
  if (!f.name) return false

  // 提取后缀名并转为小写
  const ext = f.name.substring(f.name.lastIndexOf('.')).toLowerCase()
  // 检查提取出的后缀是否存在于图片白名单中
  return imageExtensions.value.includes(ext)
}


</script>

<style lang="css" scoped>
.fillIcon {
  font-size: 5rem;
}
.image{
  max-height: 10vw;
  max-width: 10vw;
}
.maxText{
  width:clac(100% - 20px);
  overflow: hidden;
  padding: 20px;
}
</style>
