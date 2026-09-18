import type { FileApi, FileDto, UpdateTodoCommand } from "@/api"
import type { FileStore } from "@/stores/useFileStore"

type UpdateFileCommand = { uID: string, file: Blob }
export class FileContentUpdateWorkflow {

  constructor(
    private readonly file: FileDto,
    private readonly uploadFile: File,
    private readonly api: FileApi,
    private readonly store: FileStore
  ) { }

  async run(): Promise<FileDto> {
    this.PreCheck()
    const command = this.buildCommand()
    const result = await this.execute(command)

    const dto = await this.validate(result)

    this.commit(dto)
    return dto
  }

  /**
   * flow 初始化检查
   */
  private PreCheck() {
    if (this.uploadFile.name !== (this.file.fileName!+this.file.fileType)) {
      throw new Error(
        `只能上传与原文件同名的文件：${this.file.fileName}`
      )
    }
  }
  private buildCommand(): UpdateFileCommand {
    if (!this.file.uid) throw new Error("无效的ID")
    return { uID: this.file.uid, file: this.uploadFile }
  }
  private async execute(command: UpdateFileCommand) {
    return await this.api.apiFilePatch(command);
  }
  private async validate(result: string) {
    const latest = await this.api.apiFileGet({ uID: this.file.uid })
    if (latest[0].fileHash == this.file.fileHash) throw Error("File 更新失败")
    return latest[0]
  }
  private async commit(result: FileDto) {
    const index = this.store.fileList.findIndex((f: FileDto) => f.uid === result.uid)
    if (index == -1) throw new Error('File Tag 更新失败')
    this.store.fileList[index] = result
  }

}
