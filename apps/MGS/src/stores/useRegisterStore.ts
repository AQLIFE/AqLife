import { reactive, markRaw } from 'vue'
import { defineStore } from 'pinia'
import { Edit, Finished, Picture, Share } from '@element-plus/icons-vue'
import type { StepProps } from 'element-plus'

export type StepStatus = NonNullable<StepProps['status']>

interface StepItem {
  title: string
  status: StepStatus
}

export const useRegisterStore = defineStore('register', () => {
  const steps = reactive([
    { title: '填写账户基础信息', icon: markRaw(Edit), status: 'process', data: {} },
    { title: '设置账户头像', icon: markRaw(Picture), status: 'wait', data: {} },
    { title: '填写个人主页', icon: markRaw(Share), status: 'wait', data: {} },
    { title: '确认个人信息', icon: markRaw(Finished), status: 'wait', data: {} },
  ])

  return { steps }
})
