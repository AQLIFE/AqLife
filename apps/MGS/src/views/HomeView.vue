<template>
  <ElRow align="middle">
    <ElEmpty description="欢迎登录" :image-size="500" class="fill">
      <template #image>
        <BlogLogo />
      </template>
    </ElEmpty>
  </ElRow>
</template>

<script setup lang="ts">
import { ElEmpty,ElRow } from 'element-plus'
import BlogLogo from '@/assets/illustration/BlogLogo.vue'
</script>

<style lang="css" scoped>
.fill{
  height: 100vh;
}
</style>
