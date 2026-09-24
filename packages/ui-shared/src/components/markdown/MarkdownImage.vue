<template>
  <ElCol id="ImageBlock">
    <ElImage class="image" :src="src" :alt="alt" :preview-src-list="[src]" preview-teleported fit="contain" />
    <ElCol class="title">demo</ElCol>
  </ElCol>
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
#ImageBlock{
  max-width: 35vw;
}
.image {
  width: inherit;
  max-height: 100%;
}
.title{
  height:30px;
  line-height: 30px;
  text-align: center;
  width:inherit;
  background-color: aliceblue;
  margin-top: -4px;
}
</style>