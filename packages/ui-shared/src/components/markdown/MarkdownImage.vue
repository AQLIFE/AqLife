<template>
  <div class="imageContainer">
    <div id="ImageBlock">
      <ElImage
        class="image"
        :src="src"
        :alt="alt"
        :preview-src-list="[src]"
        :initial-index="0"
        :zoom-rate="1.2"
        :max-scale="5"
        :min-scale="0.2"
        preview-teleported
        fit="contain"
      />
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
  width: inherit;
  max-height: 100%;
  cursor: zoom-in;
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