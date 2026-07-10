<template>
  <ElImage
    v-if="imgSrc"
    :src="imgSrc"
    :preview-src-list="[imgSrc]"
    preview-teleported
    fit="cover"
    style="width: 50px; height: 50px; border-radius: 4px;"
  >
    <template #error>
      <div class="err-placeholder">不支持</div>
    </template>
  </ElImage>

  <div v-else-if="loading" class="loading-placeholder">加载中...</div>
  <div v-else class="err-placeholder">失败</div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, watch } from 'vue'
import { ElImage } from 'element-plus'

// 接收来自父组件表格的 uid
const props = defineProps<{
  imgSrc: string|undefined
}>()

const loading = ref(true)

const loadImage = async () => {
  if (!props.imgSrc) return
  loading.value = true
}

// 组件挂载时加载图片
onMounted(loadImage)

</script>

<style scoped>
.loading-placeholder, .err-placeholder {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 50px;
  height: 50px;
  background: #f5f7fa;
  color: #909399;
  font-size: 12px;
  border-radius: 4px;
}
</style>
