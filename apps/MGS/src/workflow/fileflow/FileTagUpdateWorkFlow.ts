import type { FileApi, FileDto, TagDto, UpdateFileTagCommand } from "@/api"
import type { FileStore } from '@/stores/useFileStore.ts'

export class FileTagUpdateWorkflow {
  constructor(
    private readonly file: FileDto,
    private readonly api: FileApi,
    private readonly store: FileStore
  ) { }
  async run() {

    const tagIds = this.extractTags(this.file.tags!)

    const command = this.buildCommand(tagIds)

    const result = await this.execute(command)

    const validateDto = await this.validate(result)

    this.commit(validateDto)
  }

  private extractTags(tags: TagDto[]): string[] {
    return tags.map(x => x.uid).filter((uid): uid is string => !!uid)
  }

  private buildCommand(tagIds: string[]) {
    return {
      uid: this.file.uid,
      tags: tagIds
    }
  }

  private async execute(command: UpdateFileTagCommand) {
    return await this.api.apiFileTagPatch({ updateFileTagCommand: command })
  }

  private async validate(result: string) {
    const latestDtos = await this.api.apiFileGet({ uID: result })
    if (!latestDtos || latestDtos.length === 0) {
      throw new Error('File Tag 更新失败')
    } else return latestDtos[0]
  }

  private commit(result: FileDto) {
    const index = this.store.fileList.findIndex((f: FileDto) => f.uid === result.uid)
    if(index==-1)throw new Error('File Tag 更新失败')
    this.store.fileList[index] = result
  }
}
