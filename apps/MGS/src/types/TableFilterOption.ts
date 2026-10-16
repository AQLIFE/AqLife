import { Brush, Promotion, Timer } from "@element-plus/icons-vue"
import { FileStatus } from "@/api"
import type { Component } from "vue"

export type TableFilterOption = {
  label: string
  value: FileStatus
  icon?: Component
}

export const publishStatusOptions: TableFilterOption[] = [
  { label: '草稿', icon: Brush,     value: FileStatus.Draft },
  { label: '预约', icon: Timer,     value: FileStatus.Scheduled },
  { label: '发布', icon: Promotion, value: FileStatus.Published }
]
