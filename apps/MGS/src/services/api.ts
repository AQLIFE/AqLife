import { createApiConfiguration, resolveApiBasePath, tokenStore } from '@aqlife/api-client'

export const apiConfiguration = createApiConfiguration({
  basePath: resolveApiBasePath(import.meta.env.VITE_API),
  getToken: () => tokenStore.get(),
  onUnauthorized: () => {
    tokenStore.clear()
    if (window.location.pathname !== '/login') {
      window.location.assign('/login')
    }
  },
  onForbidden: () => {
    tokenStore.clear()
  },
})
