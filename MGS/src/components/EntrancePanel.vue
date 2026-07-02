<template>
  <ElRow>
    <ElCol>
      <ElText>个人博客后台管理系统</ElText>
    </ElCol>
    <ElCol>
      <ElButton round @click="router.push('/login')">Login</ElButton>
    </ElCol>
    <ElCol>
      <ElButton round type="success" @click="router.push('/register')">Register</ElButton>
    </ElCol>
    <ElCol>
      <el-divider />
    </ElCol>
    <ElCol>
      <ElText>
        有疑问?看看
        <ElLink>FAQ</ElLink>
      </ElText>
    </ElCol>
  </ElRow>
</template>

<script setup lang="ts">
import router from '@/router';
import { ElCol, ElButton, ElDivider, ElRow,ElLink,ElText } from 'element-plus'

</script>

<style lang="css" scoped>
.el-col > * {
  width: 60%;
  margin: 10px;
}

.el-divider{
  margin: auto;
}
</style>
