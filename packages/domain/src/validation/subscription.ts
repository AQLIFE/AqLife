import type { SubscriptionDto } from '@aqlife/api-contract'
import { validationFail, validationOk, type ValidationResult } from './types'

export function validateHttpsLink(link?: string | null): ValidationResult {
  const value = link?.trim() ?? ''
  if (!value) return validationFail('订阅链接不能为空')
  if (!/^https:\/\/.+/i.test(value)) {
    return validationFail('订阅链接必须以 https:// 开头')
  }
  return validationOk()
}

export function validateSubscriptionItem(
  item: Pick<SubscriptionDto, 'aliasName' | 'subscriptionLink' | 'subscriptionPlatform'>,
): ValidationResult {
  const alias = item.aliasName?.trim() ?? ''
  const platform = item.subscriptionPlatform?.trim() ?? ''

  if (!alias) return validationFail('平台账户名不能为空')
  if (!platform) return validationFail('平台名称不能为空')

  return validateHttpsLink(item.subscriptionLink)
}

export function validateSubscriptionPlatformsUnique(
  items: Array<Pick<SubscriptionDto, 'subscriptionPlatform'>>,
): ValidationResult {
  const platforms = items
    .map((item) => item.subscriptionPlatform?.trim())
    .filter((name): name is string => Boolean(name))

  const unique = new Set(platforms)
  if (unique.size !== platforms.length) {
    return validationFail('不允许重复的订阅平台名称')
  }

  return validationOk()
}

export function validateSubscriptionList(
  items: Array<Pick<SubscriptionDto, 'aliasName' | 'subscriptionLink' | 'subscriptionPlatform'>>,
): ValidationResult {
  if (!items.length) return validationOk()

  for (const item of items) {
    let result = validateSubscriptionItem(item)
    if (!result.ok) return result
    result = validateHttpsLink(item.subscriptionLink) 
    if (!result.ok) return result
  }

  return validateSubscriptionPlatformsUnique(items)
}

// export function validateSubscriptionUploadIcons(
//   items: SubscriptionFullDto[],
// ): ValidationResult {
//   for (const item of items) {
//     if (!item.newIconFile && !item.subscriptionIcon) {
//       return validationFail('配置账户的图像文件缺失')
//     }
//   }
//   return validationOk()
// }
