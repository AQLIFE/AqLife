
import type { FileApi, FileDto } from "@/api"
import type { FileStore } from "@/stores/useFileStore"

type UpdateFileCommand = { uID: string, file: Blob }

export class FileContentUpdateWorkflow {
  constructor(
    private readonly file: FileDto,
    private readonly uploadFile: File,
    private readonly api: FileApi,
    private readonly store: FileStore
  ) {}

  async run(): Promise<FileDto> {
    this.preCheck()
    const result = await this.execute(this.buildCommand())
    const dto = await this.validate(result)
    this.commit(dto)
    return dto
  }

  private preCheck() {
    if (this.uploadFile.name !== (this.file.fileName! + this.file.fileType)) {
      throw new Error(`只能上传与原文件同名的文件：${this.file.fileName}`)
    }
  }

  private buildCommand(): UpdateFileCommand {
    if (!this.file.uid) throw new Error("无效的ID")
    return { uID: this.file.uid, file: this.uploadFile }
  }

  private async execute(command: UpdateFileCommand) {
    return this.api.apiFilePatch(command)
  }

  private async validate(result: string) {
    const latest = await this.api.apiFileGet({ uID: this.file.uid })
    const latestFile = latest.items?.[0]

    if (!latestFile || latestFile.fileHash === this.file.fileHash) {
      throw new Error("File 更新失败")
    }

    return latestFile
  }

  private commit(result: FileDto) {
    const index = this.store.fileList.findIndex(f => f.uid === result.uid)
    if (index === -1) throw new Error('File 更新失败')
    this.store.fileList[index] = result
  }
}
