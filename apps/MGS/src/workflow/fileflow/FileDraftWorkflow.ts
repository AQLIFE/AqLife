import type { FileApi, FileDto } from "@/api"
import type { FileStore } from "@/stores/useFileStore"

/**
 * 默认为立即发布
 */
export class FileDraftWorkflow {
  constructor(
    private readonly file: FileDto,
    private readonly api: FileApi,
    private readonly store: FileStore
  ) { }

  async run(): Promise<FileDto> {
    const uid = this.file.uid

    if (!uid) {
      throw new Error('无效的文件 ID')
    }

    const result = await this.api.apiFileCancelSchedulePatch({cancelScheduledFileCommand:{uid}})
    const cloudFile = await this.api.apiFileGet({uID:result});
    this.commit(cloudFile[0])
    return cloudFile[0]
  }

  private commit(file: FileDto) {
    this.store.replaceFile(file)
  }
}
