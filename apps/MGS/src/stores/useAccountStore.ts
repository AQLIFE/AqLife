import { ref } from 'vue'
import { defineStore } from 'pinia'
import { tokenStore } from '@aqlife/api-client'
import type { AccountDto } from '@/api'

export const useAccountStore = defineStore('account', () => {
  const bearerToken = ref<string | null>(tokenStore.get())
  const systemAccount = ref<AccountDto|undefined>()

  function setToken(token: string | null) {
    tokenStore.set(token)
    bearerToken.value = tokenStore.get()
  }

  function clearToken() {
    tokenStore.clear()
    bearerToken.value = null
  }

  return { bearerToken, setToken, clearToken,systemAccount }
})
