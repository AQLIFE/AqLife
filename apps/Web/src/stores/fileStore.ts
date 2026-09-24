import type { FileDto } from '@/api'
import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useBlogStore = defineStore('blog', () => {
  const isShow = ref(false)
  const cacheBlogList = ref<FileDto[]>([])

  function setBlogList(files: FileDto[]) {
    cacheBlogList.value = files
  }

  function clearBlogList() {
    cacheBlogList.value = []
  }

  return {
    isShow,
    cacheBlogList,
    setBlogList,
    clearBlogList,
  }
})
