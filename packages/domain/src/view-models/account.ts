import type { AccountDto, SubscriptionDto } from '@aqlife/api-contract'

export interface SubscriptionViewModel {
  aliasName: string
  platform: string
  link: string
  iconId: string | null
  iconPreviewUrl: string | null
}

export interface AccountViewModel {
  name: string
  bio: string
  avatarId: string | null
  avatarPreviewUrl: string | null
  subscriptions: SubscriptionViewModel[]
}

export interface FilePreviewOptions {
  baseUrl: string
  buildPreviewUrl: (params: { uid?: string | null; title?: string | null }) => string
}

export const defaultFilePreviewOptions = (baseUrl: string): FilePreviewOptions => ({
  baseUrl,
  buildPreviewUrl: ({ uid, title }) => {
    const query = uid
      ? `uid=${encodeURIComponent(uid)}`
      : `title=${encodeURIComponent(title ?? '')}`
    return `${baseUrl.replace(/\/+$/, '')}/api/file/preview?${query}`
  },
})

export function toSubscriptionViewModel(
  dto: SubscriptionDto,
  preview?: FilePreviewOptions,
): SubscriptionViewModel {
  const iconId = dto.subscriptionIcon ?? null
  return {
    aliasName: dto.aliasName?.trim() ?? '',
    platform: dto.subscriptionPlatform?.trim() ?? '',
    link: dto.subscriptionLink?.trim() ?? '',
    iconId,
    iconPreviewUrl:
      iconId && preview
        ? preview.buildPreviewUrl({ uid: iconId })
        : null,
  }
}

export function toAccountViewModel(
  dto: AccountDto,
  preview?: FilePreviewOptions,
): AccountViewModel {
  const avatarId = dto.avatar ?? null

  return {
    name: dto.name?.trim() ?? '',
    bio: dto.desc?.trim() ?? '',
    avatarId,
    avatarPreviewUrl:
      avatarId && preview
        ? preview.buildPreviewUrl({ uid: avatarId })
        : null,
    subscriptions: (dto.subscriptions ?? []).map((item) => toSubscriptionViewModel(item, preview)),
  }
}

export function emptyAccountViewModel(): AccountViewModel {
  return {
    name: '',
    bio: '',
    avatarId: null,
    avatarPreviewUrl: null,
    subscriptions: [],
  }
}
