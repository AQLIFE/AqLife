import type { FileDto, TagDto } from '@aqlife/api-contract'

export interface FileListItemViewModel {
  uid: string | null
  title: string
  tags: TagDto[]
  sizeLabel: string
  uploadedAt: string
}

function formatBytes(size = 0): string {
  if (size < 1024) return `${size} B`
  if (size < 1024 * 1024) return `${(size / 1024).toFixed(1)} KB`
  return `${(size / (1024 * 1024)).toFixed(1)} MB`
}

export function toFileListItemViewModel(dto: FileDto): FileListItemViewModel {
  return {
    uid: dto.uid ?? null,
    title: dto.fileName?.trim() ?? '未命名文件',
    tags: dto.tags ?? [],
    sizeLabel: formatBytes(dto.fileSize ?? 0),
    uploadedAt: dto.uploadTime ?? '',
  }
}

export function toFileListViewModel(items: FileDto[]): FileListItemViewModel[] {
  return items.map(toFileListItemViewModel)
}
