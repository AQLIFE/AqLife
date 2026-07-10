import { reactive, ref } from 'vue'
import { defineStore } from 'pinia'
import type { FileApi, FileMetadataDto } from '@/api'

export const useFileStore = defineStore('file', () => {
  const fileList = ref<FileMetadataDto[]>([])
  const previewUrl = reactive<Map<String,string>>(new Map<string, string>())
  const isInitialized = ref(false)

  // 将复杂的加载逻辑封装为 Action [cite: 145]
  async function fetchAllFiles(fileApi: FileApi) {
    if (isInitialized.value) return // 缓存命中，直接返回 [cite: 3]

    // 1. 获取元数据
    fileList.value = await fileApi.apiFileGet()

    // 2. 并行获取所有预览图，提升加载速度 [cite: 9]
    const tasks = fileList.value.map(async (item) => {
      if (!item.uid || previewUrl.has(item.uid)) return

      try {
        const blob = await fileApi.apiFilePreviewGet({ uID: item.uid })
        const url = URL.createObjectURL(blob)
        previewUrl.set(item.uid, url)
      } catch (e) {
        console.error(`加载图片[${item.uid}]失败`, e)
      }
    })

    await Promise.all(tasks)
    isInitialized.value = true
  }
  // 提供清理方法，防止内存泄漏 [cite: 139]
  function clearCache() {
    previewUrl.forEach(url => URL.revokeObjectURL(url))
    previewUrl.clear()
    isInitialized.value = false
  }

  return { fileList, previewUrl, fetchAllFiles, clearCache }
})
