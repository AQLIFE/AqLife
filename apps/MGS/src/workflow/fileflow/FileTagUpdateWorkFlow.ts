
import type { FileApi, FileDto, TagDto, UpdateFileTagCommand } from "@/api"
import type { FileStore } from '@/stores/useFileStore'

export class FileTagUpdateWorkflow {
  constructor(
    private readonly file: FileDto,
    private readonly value: TagDto[],
    private readonly api: FileApi,
    private readonly store: FileStore
  ) {}

  async run(): Promise<FileDto> {
    const tagIds = this.extractTags(this.value)
    const result = await this.execute(this.buildCommand(tagIds))
    const validateDto = await this.validate(result)
    this.commit(validateDto)
    return validateDto
  }

  private extractTags(tags: TagDto[]): string[] {
    return tags.map(x => x.uid).filter((uid): uid is string => !!uid)
  }

  private buildCommand(tagIds: string[]): UpdateFileTagCommand {
    return {
      uid: this.file.uid,
      tags: tagIds,
    }
  }

  private async execute(command: UpdateFileTagCommand) {
    return this.api.apiFileTagPatch({ updateFileTagCommand: command })
  }

  private async validate(result: string) {
    const latest = await this.api.apiFileGet({ uID: result })
    const latestFile = latest.items?.[0]

    if (!latestFile) {
      throw new Error('File Tag 更新失败')
    }

    return latestFile
  }

  private commit(result: FileDto) {
    const index = this.store.fileList.findIndex(f => f.uid === result.uid)
    if (index === -1) throw new Error('File Tag 更新失败')
    this.store.fileList[index] = result
  }
}
