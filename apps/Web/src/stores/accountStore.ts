import { defineStore } from 'pinia'
import { ref } from 'vue'

import {
  AccountApi,
  type AccountDto,
} from '@/api'

import { apiConfiguration } from '@/services/api'
import { toApiClientError } from '@aqlife/api-client'

type AccountLoadStatus =
  | 'idle'
  | 'loading'
  | 'success'
  | 'empty'
  | 'error'

export const useAuthorInfoStore = defineStore('author-info', () => {
  const userInfo = ref<AccountDto | null>(null)

  const status = ref<AccountLoadStatus>('idle')

  const errorMessage = ref<string | null>(null)

  const api =
    new AccountApi(apiConfiguration)


  const getUser = async () => {
    status.value = 'loading'
    userInfo.value = null
    errorMessage.value = null

    try {
      const response = await api.apiAccountGetRaw()

      if (response.raw.status === 204) {
        status.value = 'empty'
        return
      }

      userInfo.value = await response.value()
      status.value = 'success'
    } catch (cause: unknown) {
      // 这里再区分 ResponseError / FetchError / timeout
      status.value = 'error'
      const apiError = await toApiClientError(cause)
      errorMessage.value = apiError.message
    }
  }

  return {
    userInfo,
    status,
    errorMessage,
    getUser,
  }
})