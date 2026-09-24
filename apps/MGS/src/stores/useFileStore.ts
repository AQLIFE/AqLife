import { reactive, ref } from 'vue'
import { defineStore } from 'pinia'
import type { FileApi, FileDto } from '@/api'
import { isImageType } from '@aqlife/domain'

export type FileStore = ReturnType<typeof useFileStore>

export const useFileStore = defineStore('file', () => {
  const fileList = ref<FileDto[]>([])
  const previewUrl = reactive<Map<string, string>>(new Map())
  const isInitialized = ref(false)

  async function fetchAllFiles(fileApi: FileApi) {
    if (isInitialized.value) return

    let page = 1
    const pageSize = 10

    while (true) {
      const result = await fileApi.apiFileGet({
        page,
        pageSize,
      })

      fileList.value.push(...(result.items ?? []))

      if (!result.hasMore) break
      page = (result.page ?? page) + 1
    }

    const tasks = fileList.value.map(async item => {
      if (!item.uid || previewUrl.has(item.uid) || !isImageType(item.fileType ?? '')) return

      try {
        const blob = await fileApi.apiFilePreviewGet({ uID: item.uid })
        previewUrl.set(item.uid, URL.createObjectURL(blob))
      } catch (e) {
        console.error(`加载图片[${item.uid}]失败`, e)
      }
    })

    await Promise.all(tasks)
    isInitialized.value = true
  }

  function clearCache() {
    previewUrl.forEach(url => URL.revokeObjectURL(url))
    previewUrl.clear()
    fileList.value = []
    isInitialized.value = false
  }

  function replaceFile(file: FileDto) {
    const index = fileList.value.findIndex(x => x.uid === file.uid)

    if (index === -1) {
      throw new Error('File not found')
    }

    fileList.value[index] = file
  }

  function removeFile(uid: string) {
    const index = fileList.value.findIndex(x => x.uid === uid)

    if (index !== -1) {
      fileList.value.splice(index, 1)
    }

    const preview = previewUrl.get(uid)

    if (preview) {
      URL.revokeObjectURL(preview)
      previewUrl.delete(uid)
    }
  }

  return {
    fileList,
    previewUrl,
    fetchAllFiles,
    clearCache,
    replaceFile,
    removeFile,
  }
})
