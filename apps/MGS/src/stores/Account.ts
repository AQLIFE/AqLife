import { ref, computed } from 'vue'
import { defineStore } from 'pinia'

export const useAccountStore = defineStore('Account', () => {
  const bearerToken = ''
  const subscript = []
  return { bearerToken }
})
