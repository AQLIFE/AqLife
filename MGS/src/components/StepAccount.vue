<template>
  <ElForm label-width="120px">
    <ElFormItem label="账户名">
      <ElInput clearable placeholder="请输入账户名" />
    </ElFormItem>
    <ElFormItem label="说说">
      <ElInput clearable placeholder="请输入简介" />
    </ElFormItem>
    <ElFormItem label="系统密钥">
      <ElInput clearable type="textarea" placeholder="请输入用于初始化博客系统的密钥" />
    </ElFormItem>
    <ElFormItem>
      <ElButton type="primary" @click="$emit('next')">Next</ElButton>
    </ElFormItem>
  </ElForm>
</template>

<script setup lang="ts">
import { ElForm,ElFormItem,ElInput,ElButton } from 'element-Plus';
defineEmits(['next'])
</script>
<style lang="css" scoped>
.el-form :deep(.el-form-item__label) {
  text-align: justify;
  text-align-last: justify;

  /* 如果你的 Element Plus 默认给 label 加了 padding-right，
     文字太靠右的话，可以适当调整 padding */
  padding-right: 12px;
}
</style>
