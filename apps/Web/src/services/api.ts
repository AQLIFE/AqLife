import { createApiConfiguration, resolveApiBasePath } from '@aqlife/api-client'

export const apiConfiguration = createApiConfiguration({
  basePath: resolveApiBasePath(import.meta.env.VITE_API),
})
