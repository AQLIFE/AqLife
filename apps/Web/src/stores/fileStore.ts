import type { FileDto } from '@/api'
import { defineStore } from 'pinia'
import { reactive, ref, type Ref } from 'vue'

export const useBlogStore = defineStore('blog', () => {
  const isShow: Ref<boolean> = ref(false)
  const cacheBlogList = reactive<FileDto[]>([])

  return { isShow,cacheBlogList }
})
