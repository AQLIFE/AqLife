import { defaultFilePolicy, getUploadLimitBytes, normalizeExtension, type FilePolicyOptions } from '../policy/filePolicy'
import { validationFail, validationOk, type ValidationResult } from './types'

export function validateUploadExtension(
  fileName: string,
  policy: FilePolicyOptions = defaultFilePolicy,
): ValidationResult {
  const ext = normalizeExtension(fileName)
  if (!ext) return validationFail('文件缺少扩展名')
  if (!policy.allowedUpload.includes(ext)) {
    return validationFail(`不允许上传的文件类型：${ext}`)
  }
  return validationOk()
}

export function validateUploadSize(
  size: number,
  policy: FilePolicyOptions = defaultFilePolicy,
): ValidationResult {
  const limit = getUploadLimitBytes(policy)
  if (size > limit) {
    return validationFail(`文件大小超出限制 (最大 ${limit} B，当前 ${size} B)`)
  }
  return validationOk()
}

export function validateUploadFile(
  file: Pick<File, 'name' | 'size'>,
  policy: FilePolicyOptions = defaultFilePolicy,
): ValidationResult {
  const extResult = validateUploadExtension(file.name, policy)
  if (!extResult.ok) return extResult
  return validateUploadSize(file.size, policy)
}

export function validateDownloadExtension(
  fileName: string,
  policy: FilePolicyOptions = defaultFilePolicy,
): ValidationResult {
  const ext = normalizeExtension(fileName)
  if (!policy.allowedDownload.includes(ext)) {
    return validationFail(`不允许下载的文件类型：${ext || '(无扩展名)'}`)
  }
  return validationOk()
}
