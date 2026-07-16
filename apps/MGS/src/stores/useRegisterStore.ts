import { reactive, markRaw,type Reactive,type Ref,ref } from 'vue'
import { defineStore } from 'pinia'
import { Edit, Finished, Picture, Share } from '@element-plus/icons-vue'
import type { StepProps } from 'element-plus'
import type { AccountDto, CreateAccountCommand, SubscriptionDto } from '@/api'
// import type { ISimpleAccountInfo } from '@aqlife/api-contract'

export type StepStatus = NonNullable<StepProps['status']>

interface StepItem {
  title: string
  status: StepStatus
}

const profile = reactive<CreateAccountCommand>({})
const avatar:Ref<File|null> = ref<File|null>(null)
const subscriptions = reactive<SubscriptionDto[]>([
    {aliasName:'',subscriptionLink:'',subscriptionPlatform:'',subscriptionIcon:''}
])
const key = ref<string>('')
const PreviewUrls = reactive<Map<string,string>>(new Map())
const fileList = ref<File[]>([])
// const loginPwd = ref<string>('')

export const useRegisterStore = defineStore('register', () => {
  const steps = reactive([
    { title: '填写账户基础信息', icon: markRaw(Edit), status: 'process'},
    { title: '设置账户头像', icon: markRaw(Picture), status: 'wait'},
    { title: '填写个人主页', icon: markRaw(Share), status: 'wait'},
    { title: '确认个人信息', icon: markRaw(Finished), status: 'wait'},
  ])

  return { steps,key,profile,avatar,subscriptions,PreviewUrls,fileList}
})
