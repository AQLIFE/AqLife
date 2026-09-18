import type { FileApi, FileDto } from "@/api"
import type { FileStore } from "@/stores/useFileStore"

export class FileDeleteWorkflow {
  constructor(
    private readonly file: FileDto,
    private readonly api: FileApi,
    private readonly store: FileStore
  ) { }

  async run(): Promise<void> {
    const uid = this.file.uid

    if (!uid) {
      throw new Error('无效的文件 ID')
    }

    await this.api.apiFileDelete({
      deleteFileCommand: {
        uid
      }
    })

    this.commit(uid)
  }

  private commit(uid: string) {
    this.store.removeFile(uid)
  }
}
