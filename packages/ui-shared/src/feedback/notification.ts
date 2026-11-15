import { ElNotification } from 'element-plus'

export type AppNotificationType = 'success' | 'info' | 'warning' | 'error'

const defaults = {
  position: 'top-right',
  duration: 3600,
  showClose: true,
  grouping: true,
  offset: 24,
} as const

function show(type: AppNotificationType, message: string, title?: string) {
  return ElNotification({
    ...defaults,
    type,
    title: title ?? ({
      success: '操作成功',
      info: '提示',
      warning: '请注意',
      error: '操作失败',
    } satisfies Record<AppNotificationType, string>)[type],
    message,
  })
}

export const notify = {
  success: (message: string, title?: string) => show('success', message, title),
  info: (message: string, title?: string) => show('info', message, title),
  warning: (message: string, title?: string) => show('warning', message, title),
  error: (message: string, title?: string) => show('error', message, title),
  fromError: (error: unknown, fallback: string, title?: string) =>
    show('error', error instanceof Error && error.message ? error.message : fallback, title),
}
