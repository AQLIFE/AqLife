// stores/articleStore.ts

import type { TocNode } from '@aqlife/domain'
import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useArticleStore = defineStore('article', () => {
  const markdown = ref('')
  const blogTitle = ref('')
  const toc = ref<TocNode[]>([])

  return {
    markdown,
    blogTitle,
    toc,
  }
})