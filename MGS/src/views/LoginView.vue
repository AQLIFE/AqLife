<script setup lang="ts">
import { ElCol, ElForm, ElFormItem, ElInput, ElButton } from 'element-plus'
</script>

<template>
  <ElCol style="display: flex;flex-direction: row;justify-content: center;">
    <ElForm style="width:30vw;">
    <ElFormItem label="账户名">
      <ElInput clearable placeholder="请输入账户名" />
    </ElFormItem>
    <ElFormItem label="密码">
      <ElInput clearable placeholder="请输入密码" show-password />
    </ElFormItem>
    <ElFormItem>
      <ElButton type="info">Forget</ElButton>
      <ElButton type="primary">Login</ElButton>
    </ElFormItem>
  </ElForm>
  </ElCol>
</template>
<style lang="css" scoped>
</style>
