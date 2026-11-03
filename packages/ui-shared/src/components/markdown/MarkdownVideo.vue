<template>
  <div class="video-container">
    <video :src="src" controls playsinline preload="metadata" />
    <div v-if="title" class="video-title">{{ title }}</div>
  </div>
</template>

<script setup lang="ts">
defineProps<{ src: string; title?: string }>()
</script>

<style scoped>
.video-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 100%;
  margin: 1rem 0;
}
video {
  display: block;
  width: min(100%, 960px);
  max-height: 70vh;
  background: #000;
  border-radius: 8px;
}
.video-title {
  max-width: 960px;
  margin-top: 6px;
  color: var(--el-text-color-secondary);
  font-size: 13px;
  text-align: center;
}
</style>
