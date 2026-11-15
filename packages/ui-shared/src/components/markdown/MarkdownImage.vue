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

@media (max-width: 768px) {
  #ImageBlock {
    max-width: 100%;   /* 铺满宽度 */
    width: 100%;
    max-height: 33vh;  /* 高度不超过视觉高度的 33% */
    height: auto;      /* 保持比例，不强制拉伸 */
  }
}

.image {
  width: inherit;
  max-height: 100%;
  cursor: zoom-in;
}

.title {
  min-height: 30px;
  max-height: 62px;
  line-height: 30px;
  text-align: center;
  width: inherit;
  background-color: aliceblue;
  margin-top: -4px;
}
</style>