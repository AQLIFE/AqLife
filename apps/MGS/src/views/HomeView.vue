<template>
  <ElRow align="middle">
    <ElEmpty description="欢迎登录" :image-size="500" class="fill">
      <template #image>
        <img :src="blogLogo" alt="Blog logo" class="logo" />
      </template>
    </ElEmpty>
  </ElRow>
</template>

<script setup lang="ts">
import { ElEmpty,ElRow } from 'element-plus'
import blogLogo from '@/assets/illustration/BlogLogo.svg'
</script>

<style lang="css" scoped>
.fill {
  height: 100vh;
}

.logo {
  max-width: 500px;
  width: 100%;
}
</style>
