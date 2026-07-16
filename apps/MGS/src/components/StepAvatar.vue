<template>
  <ElForm>
    <ElFormItem required label="头像" label-position="top">
      <ElTooltip content="请上传对应博客头像">
        <ImageUpload v-model:file="registerStore.avatar" accept=".png,.jpeg,.jpg" :src="registerStore.PreviewUrls.get(registerStore.avatar?.name??'')" @change="handleFileChange"/>
      </ElTooltip>
    </ElFormItem>
    <ElFormItem>
      <ElButton type="info"    @click="$emit('prev')" :icon="ArrowLeft"/>
      <ElButton type="primary" @click="next" :icon="ArrowRight"/>
    </ElFormItem>
  </ElForm>
</template>

<script setup lang="ts">
import { ElForm, ElFormItem, ElUpload, ElButton,ElImage,ElIcon,ElTooltip, ElMessage, type UploadFile } from 'element-plus'
import { Plus,ArrowRight,ArrowLeft } from '@element-plus/icons-vue';
import ImageUpload from './ImageUpload.vue';
import { useRegisterStore } from '@/stores/useRegisterStore.ts';
import { watch } from 'vue';
const emit = defineEmits(['prev', 'next'])
const registerStore = useRegisterStore()

function next(){
  if(registerStore.avatar==null){
    ElMessage.error('请上传头像')
  }else{
    registerStore.steps[1].status = 'finish'
    ElMessage.success('头像上传成功')
    emit('next')
  }
}

function handleFileChange(file: UploadFile) {
  registerStore.PreviewUrls.set(file.name, URL.createObjectURL(file.raw as File))
  console.log('Avatar file changed:', file.name, 'Preview URL:', registerStore.PreviewUrls.get(file.name))
}
</script>
