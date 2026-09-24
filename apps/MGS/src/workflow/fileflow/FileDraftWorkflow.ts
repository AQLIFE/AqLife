
import type { FileApi, FileDto } from "@/api"
import type { FileStore } from "@/stores/useFileStore"

export class FileDraftWorkflow {
  constructor(
    private readonly file: FileDto,
    private readonly api: FileApi,
    private readonly store: FileStore
  ) {}

  async run(): Promise<FileDto> {
    const uid = this.file.uid
    if (!uid) throw new Error('无效的文件 ID')

    const result = await this.api.apiFileCancelSchedulePatch({
      cancelScheduledFileCommand: { uid },
    })
    const latest = await this.api.apiFileGet({ uID: result })
    const cloudFile = latest.items?.[0]

    if (!cloudFile) throw new Error('File 更新失败')

    this.commit(cloudFile)
    return cloudFile
  }

  private commit(file: FileDto) {
    this.store.replaceFile(file)
  }
}
