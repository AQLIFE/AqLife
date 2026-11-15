import { FileStatus, type FileApi, type FileDto } from "@/api"
import type { FileStore } from "@/stores/useFileStore"

export class FileScheduledWorkflow {
  constructor(
    private readonly file: FileDto,
    private readonly api: FileApi,
    private readonly store: FileStore
  ) {}

  async run(): Promise<FileDto> {
    const uid = this.file.uid
    if (!uid) throw new Error('无效的文件 ID')

    const scheduledAt = this.file.publishAt
    if (!scheduledAt) throw new Error('请先设置预定发布时间')

    const result = await this.api.apiFileSchedulePatch({
      scheduledFileCommand: {
        uid,
        scheduledAt,
      },
    })

    if (result.toLowerCase() !== uid.toLowerCase()) {
      throw new Error('预定发布接口返回的文件 ID 不匹配')
    }

    const updatedFile: FileDto = {
      ...this.file,
      publishStatus: FileStatus.Scheduled,
      publishAt: scheduledAt,
    }

    this.store.upsertFile(updatedFile)
    return updatedFile
  }
}
