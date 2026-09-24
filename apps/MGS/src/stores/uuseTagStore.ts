import type { TagApi, TagDto } from '@/api'
import { defineStore } from 'pinia'
import { reactive } from 'vue'

export const useTagStore = defineStore('tags', () => {
  const tags = reactive<TagDto[]>([])

  async function fetchAllTags(tagApi: TagApi) {
    if (tags.length > 0) return

    let page = 1
    const pageSize = 50

    while (true) {
      const result = await tagApi.apiTagGet({
        page,
        pageSize,
      })

      tags.push(...(result.items ?? []))

      if (!result.hasMore) break
      page = (result.page ?? page) + 1
    }
  }

  return { tags, fetchAllTags }
})
