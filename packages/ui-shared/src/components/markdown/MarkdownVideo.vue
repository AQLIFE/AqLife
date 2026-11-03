<template>
  <VideoPlayer :src="src" :title="title" />
</template>

<script setup lang="ts">
import VideoPlayer from '../VideoPlayer.vue'

defineProps<{ src: string; title?: string }>()
</script>
