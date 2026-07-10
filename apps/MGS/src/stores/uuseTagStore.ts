import { reactive, markRaw } from 'vue'
import { defineStore } from 'pinia'
import { Edit, Finished, Picture, Share } from '@element-plus/icons-vue'
import type { StepProps } from 'element-plus'
import type { TagDto } from '@/api'


export const useTagStore = defineStore('tags', () => {
  const tags =reactive<TagDto[]>([])

  return { tags }
})
