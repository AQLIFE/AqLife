import { Brush, Promotion, Timer } from "@element-plus/icons-vue"
import type { Component } from "vue"

export type TableFilterOption = {
  label: string
  value: number
  icon?: Component
}

export enum publishStatus { Draft, Scheduled, Published }
export const publishStatusOptions: TableFilterOption[] = [
  { label: '草稿', icon: Brush, value: publishStatus.Draft },
  { label: '预约', icon: Timer, value: publishStatus.Scheduled },
  { label: '发布', icon: Promotion, value: publishStatus.Published }
]
