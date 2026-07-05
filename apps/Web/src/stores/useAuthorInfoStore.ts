import { AccountApi, type AccountDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { handle } from '@/utils/request'
import { defineStore } from 'pinia'
import { ref, type Ref } from 'vue'

export const useAuthorInfoStore = defineStore('author-info', () => {
  const userInfo = ref<AccountDto>({
    name: '',
    desc: '',
    avatar: null,
    subscriptions: [],
  })
  const isShow: Ref<boolean> = ref(true)

  const api = new AccountApi(apiConfiguration)
  const getUser = async () => {
    const [response, status] = await handle(api.apiAccountGetRaw())
    try {
      const data = await response?.value()
      if (status && data != null) {
        userInfo.value = data
        isShow.value = false
      }
    } catch {
      /* empty response */
    }
  }

  return { userInfo, isShow, getUser }
})
