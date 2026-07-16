<template>
  <ElUpload ref="uploadRef" :on-exceed="handleExceed"  v-model:file-list="fileList" action="#" :limit="1" :disabled="status" :show-file-list="false" :auto-upload="false" @change="handleChange" :accept="props.accept">
    <ElImage :lazy="true" :src="src??''" :style="{ width: iconSize, height:iconSize }">
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
import { ElImage, ElMessage, ElUpload, ElIcon, type UploadFile, type UploadRawFile, genFileId, type UploadInstance } from 'element-plus'
const props = defineProps({
  status: {
    type: Boolean,
    default: false,
    required:false
  },
  iconSize: {
    type: String,
    default: '8vw',
    required: false
  },
  src:{
    type:String,
    default:'',
    required:false
  },
  accept:{
    type:String,
    default:'.svg',
    required:false
  }
})
const modelValue = defineModel<File | null>('file')

  const fileList = ref([])
const uploadRef = ref<UploadInstance>()
const emit = defineEmits(['change'])

function handleExceed(files: File[]) {
  URL.revokeObjectURL(props.src as string)
  const file = files as UploadRawFile[]
  file[0].uid = genFileId() // 生成新 ID 触发更新
  uploadRef.value!.handleStart(file[0]) // 手动启动新文件的处理流程，这会触发 handleChange
}

function handleChange(file: UploadFile) {
  if(props.src!='')
    URL.revokeObjectURL(props.src as string)

  modelValue.value = file.raw as File
  emit('change', file)
  ElMessage.info(`Selected file: ${file.name}`)
}
</script>

<style lang="css" scoped>

</style>
