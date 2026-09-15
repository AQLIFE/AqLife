<template>
  <ElUpload ref="uploadRef" @exceed="handleExceed" @change="handleChange" v-model:file-list="PrivateFileQueue"
  action="#" :limit="1" :disabled="disabled" :show-file-list="false" :auto-upload="false" :accept="props.accept" title="点击图标上传">
    <ElImage :src="src??''" :style="{ width: iconSize, height:iconSize }">
      <template #error>
        <ElIcon :style="{ fontSize: props.iconSize }">
          <slot>
            <Plus />
          </slot>
        </ElIcon>
      </template>
    </ElImage>
    <template #tip>
      <slot name="tip"></slot>
    </template>
  </ElUpload>
</template>

<script lang="ts" setup>
import { Plus } from '@element-plus/icons-vue'
import { ref } from 'vue'
import { ElImage, ElMessage, ElUpload, ElIcon, type UploadFile, type UploadRawFile, genFileId, type UploadInstance, type UploadUserFile } from 'element-plus'
const props = defineProps({
  disabled: {
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
const emit = defineEmits(['change'])
const modelValue = defineModel<File|null>('file')// 交由外部组件跟踪的文件

const PrivateFileQueue = ref<UploadUserFile[]>([])
const uploadRef = ref<UploadInstance>()

function handleExceed(files: File[]):void {
  // URL.revokeObjectURL(props.src as string) 取消该职能设置
  const file = files as UploadRawFile[]
  file[0].uid = genFileId() // 生成新 ID 触发更新
  uploadRef.value!.handleStart(file[0]) // 手动启动新文件的处理流程，这会触发 handleChange
}

function handleChange(uploadFile: UploadFile):void {
  // if(props.src!='')
  //   URL.revokeObjectURL(props.src as string)

  console.log("old=>"+modelValue.value?.name,'\t',"new=>"+uploadFile.name)
  modelValue.value = uploadFile.raw
  emit('change', uploadFile)
  ElMessage.success(`Selected file: ${uploadFile.name}`)
}
</script>
