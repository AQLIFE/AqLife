import type { FileDto } from '@/api'
import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useBlogStore = defineStore('blog', () => {
  const hasMore = ref(true)
  const page = ref(1)
  const categoryUID = ref<string>()

  const cacheBlogList = ref<FileDto[]>([])

  function clearBlogList() {
    cacheBlogList.value = []
  }
   function setBlogList(files: FileDto[]) {
    cacheBlogList.value = files
  }

  function reset(category?: string) {
    categoryUID.value = category
    page.value = 1
    hasMore.value = true
    cacheBlogList.value = []
  }

  return {
    hasMore,
    page,
    categoryUID,
    cacheBlogList,
    reset,
    clearBlogList,
    setBlogList
  }
})
