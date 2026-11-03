import type { TocNode } from '@aqlife/domain'
import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useArticleStore = defineStore('article', () => {
  const markdown = ref('')
  const blogTitle = ref('')
  const toc = ref<TocNode[]>([])
  const activeAnchor = ref('')
  const isFullscreen = ref(false)

  return {
    markdown,
    blogTitle,
    toc,
    activeAnchor,
    isFullscreen,
  }
})