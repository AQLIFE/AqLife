<template>
  <ElUpload  action="#" :limit="1" :disabled="status" :show-file-list="false" :auto-upload="false" @change="handleChange">
    <ElImage :lazy="true" :src="imageSrc ||props.url" :style="{ width: iconSize, height:iconSize }">
      <template #error>
        <ElIcon :style="{ fontSize: props.iconSize }">
          <Plus />
        </ElIcon>
      </template>
    </ElImage>
  </ElUpload>
</template>

<script lang="ts" setup>
import { Plus } from '@element-plus/icons-vue'
import { ref } from 'vue'
import { ElImage, ElMessage, ElUpload, ElIcon, type UploadFile } from 'element-plus'
const props = defineProps({
  status: {
    type: Boolean,
    default: false
  },
  iconSize: {
    type: String,
    default: '8vw',
    required: false
  },
  url: {
    type: String,
    default: '',
    required: false
  }
})
const modelValue = defineModel<File | null>()


const imageSrc = ref<string>('')

function handleChange(file: UploadFile) {
  //  const file = file.raw as File;
  imageSrc.value = URL.createObjectURL(file.raw as File)
  modelValue.value = file.raw as File
  ElMessage.info(`Selected file: ${file.name}`)
  // 这里可以添加上传逻辑，例如调用 API 上传图片
}
</script>

<style lang="css" scoped>

</style>
