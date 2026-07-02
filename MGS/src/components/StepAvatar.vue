<template>
  <ElForm>
    <ElFormItem>
      <ElTooltip content="请上传对应博客头像">
        <ElUpload :disabled="isUploaded" :auto-upload="false" :limit="1" :show-file-list="false" action="#">
          <ElImage src="" fit="cover" style="   width: 10vw;   height: 10vw;   display: block;   border-radius: 4px;   border: 1px dashed #d9d9d9; ">
            <template #error>
              <ElIcon style="position: relative; top: 1vw; font-size: 8vw">
                <Plus />
              </ElIcon>
            </template>
          </ElImage>
        </ElUpload>
      </ElTooltip>
    </ElFormItem>
    <ElFormItem>
      <ElButton type="info"    @click="$emit('prev')">Previous</ElButton>
      <ElButton type="primary" @click="$emit('next')">Next</ElButton>
    </ElFormItem>
  </ElForm>
</template>

<script setup lang="ts">
import { ElForm, ElFormItem, ElUpload, ElButton,ElImage,ElIcon,ElTooltip } from 'element-plus'
import { Plus } from '@element-plus/icons-vue';
defineEmits(['prev', 'next'])
</script>
