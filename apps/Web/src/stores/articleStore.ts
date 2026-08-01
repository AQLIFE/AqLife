// stores/articleStore.ts

import { defineStore } from 'pinia'
import { ref,type Ref } from 'vue'
import { type TocNode } from '@/services/markdownParser'

export const useArticleStore = defineStore('article', () => {
    const markdown =ref('')
    const blogTitle: Ref<string> = ref('')
    const toc = ref<TocNode[]>()
    return {markdown,toc,blogTitle}
})