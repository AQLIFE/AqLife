<template>
  <figure class="video-player">
    <video
      class="video-player__media"
      :src="src"
      :poster="poster"
      :controls="controls"
      :autoplay="autoplay"
      :loop="loop"
      :muted="muted"
      :preload="preload"
      playsinline
    >
      您的浏览器不支持 HTML5 视频播放。
    </video>
    <figcaption v-if="title" class="video-player__title">{{ title }}</figcaption>
  </figure>
</template>

<script setup lang="ts">
withDefaults(defineProps<{
  src: string
  title?: string
  poster?: string
  controls?: boolean
  autoplay?: boolean
  loop?: boolean
  muted?: boolean
  preload?: 'none' | 'metadata' | 'auto'
}>(), {
  controls: true,
  autoplay: false,
  loop: false,
  muted: false,
  preload: 'metadata',
})
</script>

<style scoped>
.video-player {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 100%;
  margin: 1rem 0;
}
.video-player__media {
  display: block;
  width: min(100%, 960px);
  max-height: 70vh;
  aspect-ratio: 16 / 9;
  object-fit: contain;
  background: #000;
  border-radius: 8px;
}
.video-player__title {
  max-width: 960px;
  margin-top: 6px;
  color: var(--el-text-color-secondary, currentColor);
  font-size: 13px;
  text-align: center;
}
</style>
