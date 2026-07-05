export interface FilePolicyOptions {
  maxFileSize: number
  storageUnit: number
  allowedUpload: string[]
  allowedDownload: string[]
}

/** 与后端 FilePolicy.Development.json 默认策略对齐 */
export const defaultFilePolicy: FilePolicyOptions = {
  maxFileSize: 100,
  storageUnit: 10,
  allowedUpload: ['.md', '.svg', '.jpeg', '.jpg', '.png'],
  allowedDownload: ['.md'],
}

/** 订阅平台图标仅允许 SVG */
export const svgIconPolicy: FilePolicyOptions = {
  ...defaultFilePolicy,
  allowedUpload: ['.svg'],
}

export function getUploadLimitBytes(policy: FilePolicyOptions): number {
  return policy.maxFileSize << policy.storageUnit
}

export function normalizeExtension(fileName: string): string {
  const ext = fileName.includes('.') ? fileName.slice(fileName.lastIndexOf('.')) : ''
  return ext.toLowerCase()
}
