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
import { FileApi } from '@/api'
import { apiConfiguration } from '@/services/api'

// 接收来自父组件表格的 uid
const props = defineProps<{
  uid: string
}>()

const fileApi = new FileApi(apiConfiguration)
const imgSrc = ref('')
const loading = ref(true)

const loadImage = async () => {
  if (!props.uid) return
  loading.value = true
  try {
    // 1. 调用你的鉴权接口，注意：确保你的 OpenAPI 客户端此处返回的是 Blob 对象
    // 如果生成的客户端支持传参，可能需要确保 responseType 是 'blob'
    const blob = await fileApi.apiFilePreviewGet({ uID: props.uid })

    // 2. 将返回的二进制流（Blob）转换为浏览器可识别的临时 URL
    // const blob = response as Blob // 根据你 SDK 实际返回类型断言

    // 如果之前已经有一个 URL，先释放它
    if (imgSrc.value) {
      URL.revokeObjectURL(imgSrc.value)
    }

    imgSrc.value = URL.createObjectURL(blob)
  } catch (error) {
    console.error('图片加载失败:', error)
  } finally {
    loading.value = false
  }
}

// 组件挂载时加载图片
onMounted(loadImage)

// 监听 uid 变化（防止表格行复用时内容不刷新）
watch(() => props.uid, loadImage)

// 🔴 极其重要：组件销毁时，释放浏览器内存中的 Object URL，防止内存泄漏
onBeforeUnmount(() => {
  if (imgSrc.value) {
    URL.revokeObjectURL(imgSrc.value)
  }
})
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
