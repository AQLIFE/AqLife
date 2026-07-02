import { ref, reactive } from 'vue'
import { defineStore } from 'pinia'
import { Edit,Finished,Picture,Share } from '@element-plus/icons-vue';
import type { StepProps } from 'element-Plus'

// 抽取官方的 status 类型
export type StepStatus = NonNullable<StepProps['status']>
interface StepItem {
  title: string
  // 使用官方定义的字面量类型进行约束
  status: StepStatus
}

export const useRegisterStore = defineStore('registerStatus', () => {
  const steps = reactive([
    { title: "填写账户基础信息", icon: Edit, status: 'process',data:{}},
    { title: "设置账户头像", icon: Picture, status: 'wait',data:{} },
    { title: "填写个人主页", icon: Share, status: 'wait',data:{}},
    { title: "确认个人信息", icon: Finished, status: 'wait',data:{} },
  ])

  return { steps }
})
