import { reactive, ref } from 'vue'
import { defineStore } from 'pinia'
import { FileScope, type FileApi, type FileDto } from '@/api'
import { isImageType } from '@aqlife/domain'

export type FileStore = ReturnType<typeof useFileStore>

export const useFileStore = defineStore('file', () => {
  const fileList = ref<FileDto[]>([])
  const previewUrl = reactive<Map<string, string>>(new Map())
  const isInitialized = ref(false)
  const loadedScope = ref<FileScope | null>(null)
  let loadVersion = 0

  async function fetchAllFiles(fileApi: FileApi, scope: FileScope = FileScope.Blog) {
    if (isInitialized.value && loadedScope.value === scope) return

    if (loadedScope.value !== null && loadedScope.value !== scope) {
      clearCache()
    }

    const version = ++loadVersion
    let page = 1
    const pageSize = 50
    const files: FileDto[] = []

    while (true) {
      const result = await fileApi.apiFileGet({ page, pageSize, scope })
      if (version !== loadVersion) return

      files.push(...(result.items ?? []))
      if (!result.hasMore) break
      page = (result.page ?? page) + 1
    }

    fileList.value = files
    const tasks = files.map(async item => {
      if (version !== loadVersion || !item.uid || previewUrl.has(item.uid) || !isImageType(item.fileType ?? '')) return

      try {
        const blob = await fileApi.apiFilePreviewGet({ uID: item.uid })
        if (version !== loadVersion) return
        previewUrl.set(item.uid, URL.createObjectURL(blob))
      } catch (e) {
        console.error(`加载图片[${item.uid}]失败`, e)
      }
    })

    await Promise.all(tasks)
    if (version !== loadVersion) return

    loadedScope.value = scope
    isInitialized.value = true
  }

  function clearCache() {
    loadVersion++
    previewUrl.forEach(url => URL.revokeObjectURL(url))
    previewUrl.clear()
    fileList.value = []
    loadedScope.value = null
    isInitialized.value = false
  }

  function upsertFile(file: FileDto) {
    if (!file.uid) return

    const index = fileList.value.findIndex(x => x.uid === file.uid)

    if (index === -1) {
      fileList.value.push(file)
      return
    }

    fileList.value[index] = file
  }

  function replaceFile(file: FileDto) {
    if (!file.uid) throw new Error('File not found')

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
    upsertFile,
    replaceFile,
    removeFile,
  }
})
