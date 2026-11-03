import { computed, ref } from 'vue'
import * as monaco from 'monaco-editor'
import { FileApi } from '@/api'
import { ElMessage } from 'element-plus'

export type VideoUploadState = 'queued' | 'uploading' | 'completed' | 'failed'

export interface VideoUploadItem {
  id: string
  name: string
  size: number
  state: VideoUploadState
  progress: number
  error?: string
}

const CHUNK_SIZE = 4 * 1024 * 1024

export function useMarkdownVideoUploadQueue(getEditor: () => monaco.editor.IStandaloneCodeEditor | undefined, fileApi: FileApi) {
  const items = ref<VideoUploadItem[]>([])
  const active = new Map<string, Promise<void>>()
  const pendingFiles = new Map<string, File>()
  const pendingTokens = new Map<string, string>()
  const pendingUids = new Map<string, string>()
  let queueTail: Promise<void> = Promise.resolve()
  const busy = computed(() => items.value.some(item => item.state === 'queued' || item.state === 'uploading'))

  function replaceToken(token: string, replacement: string) {
    const model = getEditor()?.getModel()
    const editor = getEditor()
    if (!model || !editor) return false
    const matches = model.findMatches(token, false, false, false, null, true)
    if (!matches.length) return false
    return editor.executeEdits('video-upload-queue', matches.map(match => ({
      range: match.range,
      text: replacement,
      forceMoveMarkers: true,
    })))
  }

  function addFiles(files: File[]) {
    const editor = getEditor()
    if (!editor?.getModel()) return

    const valid = files.filter(file => /\.(mp4|webm)$/i.test(file.name))
    const rejected = files.filter(file => !valid.includes(file))
    if (rejected.length) throw new Error('仅支持 MP4 和 WebM 视频。')
    const selection = editor.getSelection()
    if (!selection) return

    const jobs = valid.map(file => {
      const id = crypto.randomUUID()
      const token = `![视频上传中：${file.name}](pending-video:${id})`
      const item: VideoUploadItem = { id, name: file.name, size: file.size, state: 'queued', progress: 0 }
      items.value.push(item)
      pendingFiles.set(id, file)
      pendingTokens.set(id, token)
      return token
    })
    if (!jobs.length) return

    editor.executeEdits('insert-pending-video', [{
      range: selection,
      text: jobs.join('\n') + '\n',
      forceMoveMarkers: true,
    }])
    editor.focus()

    for (const item of items.value.filter(x => jobs.some(token => token.includes(x.id)))) {
      const task = queueTail.then(() => upload(item.id))
      queueTail = task.catch(() => undefined)
      active.set(item.id, task)
      void task.catch(() => undefined).finally(() => active.delete(item.id))
    }
  }

  async function upload(id: string) {
    const item = items.value.find(x => x.id === id)
    const file = pendingFiles.get(id)
    const token = pendingTokens.get(id)
    if (!item || !file || !token) return
    item.state = 'uploading'
    item.error = undefined
    item.progress = 0
    ElMessage.info(`视频「${item.name}」开始上传`)

    try {
      let uid = pendingUids.get(id)
      if (!uid) {
        const session = await fileApi.apiFileUploadsPost({
          startRequest: { fileName: file.name, fileSize: file.size, chunkSize: CHUNK_SIZE },
        })
        if (!session.uploadId || !session.chunkSize || !session.chunkCount) throw new Error('服务端未返回有效的视频上传任务。')

        for (let index = 0; index < session.chunkCount; index++) {
          const start = index * session.chunkSize
          const chunk = file.slice(start, Math.min(file.size, start + session.chunkSize))
          await fileApi.apiFileUploadsUploadIdChunksIndexPut({
            uploadId: session.uploadId,
            index,
            chunk,
          })
          item.progress = Math.round(((index + 1) / session.chunkCount) * 90)
        }

        const uids = await fileApi.apiFileUploadsUploadIdCompletePost({ uploadId: session.uploadId })
        uid = uids[0]
        if (!uid) throw new Error('视频上传完成，但服务端未返回文件 UID。')
        pendingUids.set(id, uid)
      }

      // Make the uploaded media publicly readable like images pasted into Markdown.
      await fileApi.apiFileSchedulePatch({ scheduledFileCommand: { uid } })
      item.progress = 98

      const inserted = replaceToken(token, `![视频](${uid})`)
      if (!inserted) throw new Error('视频已上传，但正文占位符已不存在；请重新插入该视频 UID。')
      item.state = 'completed'
      item.progress = 100
      pendingFiles.delete(id)
      pendingTokens.delete(id)
      pendingUids.delete(id)
      ElMessage.success(`视频「${item.name}」上传完成`)
    } catch (error) {
      item.state = 'failed'
      item.error = error instanceof Error ? error.message : '上传失败'
      ElMessage.error(`视频「${item.name}」上传失败：${item.error}`)
      throw error
    }
  }

  async function retry(id: string) {
    if (active.has(id) || !pendingFiles.has(id)) return
    const task = queueTail.then(() => upload(id))
    queueTail = task.catch(() => undefined)
    active.set(id, task)
    try {
      await task
    } catch {
      // Failure is exposed on the corresponding queue item.
    } finally {
      active.delete(id)
    }
  }

  async function waitForAll() {
    await Promise.allSettled(Array.from(active.values()))
    const failed = items.value.filter(item => item.state === 'failed')
    const pending = items.value.filter(item => item.state !== 'completed')
    if (failed.length || pending.length) {
      throw new Error(failed.length
        ? `有 ${failed.length} 个视频上传失败，请重试后再保存博文。`
        : '视频上传尚未完成，请稍后再保存。')
    }
  }

  return { items, busy, addFiles, retry, waitForAll }
}
