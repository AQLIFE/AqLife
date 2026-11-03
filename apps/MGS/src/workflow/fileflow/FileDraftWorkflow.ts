import { FileStatus, type FileApi, type FileDto } from "@/api"
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

    if (result.toLowerCase() !== uid.toLowerCase()) {
      throw new Error('取消预定接口返回的文件 ID 不匹配')
    }

    const updatedFile: FileDto = {
      ...this.file,
      publishStatus: FileStatus.Draft,
      publishAt: null,
    }

    this.store.upsertFile(updatedFile)
    return updatedFile
  }
}
