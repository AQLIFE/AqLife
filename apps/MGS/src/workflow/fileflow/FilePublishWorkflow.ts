import { FileStatus, type FileApi, type FileDto } from "@/api"
import type { FileStore } from "@/stores/useFileStore"

export class FilePublishWorkflow {
  constructor(
    private readonly file: FileDto,
    private readonly api: FileApi,
    private readonly store: FileStore
  ) {}

  async run(): Promise<FileDto> {
    const uid = this.file.uid
    if (!uid) throw new Error('无效的文件 ID')

    // The endpoint returns the updated file UID. Do not perform a second search
    // here: a visibility/query mismatch must not turn a successful publish into
    // a user-visible failure.
    const result = await this.api.apiFileSchedulePatch({
      scheduledFileCommand: { uid },
    })

    if (result.toLowerCase() !== uid.toLowerCase()) {
      throw new Error('发布接口返回的文件 ID 不匹配')
    }

    const updatedFile: FileDto = {
      ...this.file,
      publishStatus: FileStatus.Published,
      publishAt: new Date().toISOString(),
    }

    this.store.upsertFile(updatedFile)
    return updatedFile
  }
}
