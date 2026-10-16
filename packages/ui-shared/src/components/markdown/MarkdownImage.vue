<template>
  <div class="imageContainer">
    <div id="ImageBlock">
      <ElImage class="image" :src="src" :alt="alt" :preview-src-list="[src]" preview-teleported fit="contain" />
      <ElCol v-if="title || alt" class="title">{{ title || alt }}</ElCol>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ElImage, ElCol } from 'element-plus'

defineProps<{
  src: string
  alt?: string,
  title?: string
}>()
</script>

<style scoped>

.imageContainer{
  width:100vw;
  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;
  justify-content: center;
  justify-self: center;
}
#ImageBlock {
  max-width: 35%;
}

.image {
  width:inherit;
  max-height: 100%;
}

.title {
  height: 30px;
  line-height: 30px;
  text-align: center;
  width: inherit;
  background-color: aliceblue;
  margin-top: -4px;
}
</style>