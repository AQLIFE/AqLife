// stores/articleStore.ts

import { defineStore } from 'pinia'
import { ref,type Ref } from 'vue'
import type { TagDto } from '@/api'

export const useTagStore = defineStore('tag', () => {
   const tagList:Ref<TagDto[]> = ref<TagDto[]>([])
    return {tagList}
})