<template>
  <ElForm class="form">
    <ElFormItem>
      <ElTooltip content="请上传对应博客头像">
        <ElUpload :disabled="isActive" :auto-upload="false" :limit="1" :show-file-list="false" action="#">
          <ElImage
            src=""
            fit="cover"
            style="
              width:  10vw;
              height: 10vw;
              display: block;
              border-radius: 4px;
              border: 1px dashed #d9d9d9;
            "
          >
            <template #error>
              <ElIcon style="position: relative; top: 1vw;font-size: 8vw;">
                <Plus />
              </ElIcon>
            </template>
          </ElImage>
        </ElUpload>
      </ElTooltip>
    </ElFormItem>
    <ElFormItem>
      <ElInput :disabled="isActive" />
    </ElFormItem>
    <ElFormItem>
      <ElInput :disabled="isActive"/>
    </ElFormItem>
    <ElFormItem>
      <ElButton @click="isActive=!isActive" :title="isActive?'解锁':'锁定'">
        <ElIcon><component :is="isActive?Unlock:Lock"/></ElIcon>
      </ElButton>
      <ElButton type="success">Send</ElButton>
    </ElFormItem>
  </ElForm>
</template>
<script setup lang="ts">
import { ElForm, ElFormItem,ElImage,ElIcon,ElUpload,ElTooltip,ElInput,ElButton } from 'element-plus'
import { Plus,Lock,Unlock } from '@element-plus/icons-vue';
import { ref } from 'vue';

const isActive = ref(true)
</script>

<style lang="css" scoped>
.form{
  text-align: center;
  justify-self: center;
}
</style>
