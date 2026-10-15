import { Brush, Promotion, Timer } from "@element-plus/icons-vue"
import { FileStatus } from "@/api"
import type { Component } from "vue"

export type TableFilterOption = {
  label: string
  value: FileStatus
  icon?: Component
}

export const publishStatus = FileStatus
export type PublishStatus = FileStatus
export const publishStatusOptions: TableFilterOption[] = [
  { label: '草稿', icon: Brush, value: publishStatus.Draft },
  { label: '预约', icon: Timer, value: publishStatus.Scheduled },
  { label: '发布', icon: Promotion, value: publishStatus.Published }
]
