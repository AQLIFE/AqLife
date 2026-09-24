import type { FileDto } from '@/api'
import { defineStore } from 'pinia'
import { reactive, ref } from 'vue'

export const useBlogStore = defineStore('blog', () => {
  const hasMore = ref(false)
  const lastRequestTime = reactive(Date.now)
  const cacheBlogList = ref<FileDto[]>([])

  function setBlogList(files: FileDto[]) {
    cacheBlogList.value = files
  }

  function clearBlogList() {
    cacheBlogList.value = []
  }

  return {
    hasMore,
    cacheBlogList,
    lastRequestTime,
    setBlogList,
    clearBlogList,
  }
})
