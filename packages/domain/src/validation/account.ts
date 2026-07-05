import { firstFailure, validationFail, validationOk, type ValidationResult } from './types'

const ACCOUNT_NAME_MIN = 3
const ACCOUNT_NAME_MAX = 20

export function validateAccountName(name?: string | null): ValidationResult {
  const value = name?.trim() ?? ''
  if (value.length < ACCOUNT_NAME_MIN || value.length > ACCOUNT_NAME_MAX) {
    return validationFail(`用户名长度需在 ${ACCOUNT_NAME_MIN}-${ACCOUNT_NAME_MAX} 之间`)
  }
  return validationOk()
}

export function validateSecretKey(secret?: string | null): ValidationResult {
  const value = secret?.trim() ?? ''
  if (!value) return validationFail('系统密钥不能为空')
  if (/\s/.test(value)) return validationFail('系统密钥不能包含空格')
  return validationOk()
}

export function validateLoginCommand(accountName?: string | null, secretKey?: string | null): ValidationResult {
  return firstFailure([validateAccountName(accountName), validateSecretKey(secretKey)])
}

export function validateAccountProfile(name?: string | null, desc?: string | null): ValidationResult {
  const nameResult = validateAccountName(name)
  if (!nameResult.ok) return nameResult

  if (desc != null && desc.length > 200) {
    return validationFail('简介长度不能超过 200 字')
  }

  return validationOk()
}
