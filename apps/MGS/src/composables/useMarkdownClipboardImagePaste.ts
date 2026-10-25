import { ref } from 'vue'
import { ElMessageBox } from 'element-plus'
import * as monaco from 'monaco-editor'

type ClipboardImagePasteOptions = {
  getEditor: () => monaco.editor.IStandaloneCodeEditor | undefined
  isEnabled: () => boolean
  uploadFiles: (files: File[]) => Promise<string[]>
}

const imageExtensionByMime: Record<string, string> = {
  'image/png': '.png',
  'image/jpeg': '.jpg',
  'image/jpg': '.jpg',
  'image/svg+xml': '.svg',
}

function clipboardFiles(event: ClipboardEvent): { files: File[]; sawUnsupportedImage: boolean } {
  const files: File[] = []
  let sawUnsupportedImage = false
  const data = event.clipboardData
  if (!data) return { files, sawUnsupportedImage }

  for (const item of Array.from(data.items)) {
    if (item.kind !== 'file') continue
    const mime = item.type.toLowerCase()
    if (!mime.startsWith('image/')) continue
    const file = item.getAsFile()
    if (!file) continue
    if (!imageExtensionByMime[mime]) {
      sawUnsupportedImage = true
      continue
    }
    files.push(file)
  }

  if (files.length === 0) {
    const svg = data.getData('image/svg+xml')
    if (svg.includes('<svg')) {
      files.push(new File([svg], 'clipboard-image.svg', { type: 'image/svg+xml' }))
    }
  }

  return { files, sawUnsupportedImage }
}

export function useMarkdownClipboardImagePaste(options: ClipboardImagePasteOptions) {
  const uploading = ref(false)

  async function onPaste(event: ClipboardEvent) {
    if (!options.isEnabled() || !event.clipboardData) return

    const { files, sawUnsupportedImage } = clipboardFiles(event)
    if (files.length === 0) {
      if (sawUnsupportedImage) {
        event.preventDefault()
        event.stopPropagation()
        await ElMessageBox.alert(
          '目前仅支持粘贴 PNG、JPEG、JPG 或 SVG 图片。',
          '图片格式不受支持',
          { type: 'warning', confirmButtonText: '知道了' },
        )
      }
      return
    }

    event.preventDefault()
    event.stopPropagation()

    const editor = options.getEditor()
    const selection = editor?.getSelection()
    const model = editor?.getModel()
    if (!editor || !selection || !model) return

    uploading.value = true
    editor.updateOptions({ readOnly: true })
    try {
      const normalizedFiles = files.map((file, index) => {
        const extension = imageExtensionByMime[file.type.toLowerCase()]
        const name = `clipboard-image-${crypto.randomUUID()}-${index + 1}${extension}`
        return new File([file], name, { type: file.type.toLowerCase() })
      })

      const uids = await options.uploadFiles(normalizedFiles)
      if (uids.length !== normalizedFiles.length || uids.some(uid => !uid)) {
        throw new Error('图片上传返回的 UID 数量不正确，请重试。')
      }

      const currentModel = editor.getModel()
      if (!currentModel || currentModel !== model) {
        throw new Error('编辑器内容已切换，图片已上传但未插入正文。请重新粘贴或手动插入图片 UID。')
      }

      const snippets = uids.map(uid => `![未命名图像](${uid})`).join('\n')
      editor.executeEdits('clipboard-image-paste', [{
        range: selection,
        text: snippets,
        forceMoveMarkers: true,
      }])
      editor.focus()
    } catch (error) {
      const message = error instanceof Error ? error.message : '图片上传失败，请重试。'
      await ElMessageBox.alert(message, '图片上传失败', {
        type: 'warning',
        confirmButtonText: '知道了',
      })
    } finally {
      editor.updateOptions({ readOnly: !options.isEnabled() })
      uploading.value = false
    }
  }

  return { uploading, onPaste }
}
