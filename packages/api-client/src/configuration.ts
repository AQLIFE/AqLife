import { Configuration, type ConfigurationParameters, type Middleware } from '@aqlife/api-contract'
import { tokenStore } from './token'

export interface ApiClientOptions {
  basePath?: string
  /** 返回 Bearer Token 或完整 Authorization 头值 */
  getToken?: () => string | null | Promise<string | null>
  credentials?: RequestCredentials
  onUnauthorized?: (response: Response) => void
  onForbidden?: (response: Response) => void
  extraMiddleware?: Middleware[]
  headers?: Record<string, string>
}

function formatAuthorization(token: string): string {
  return token.startsWith('Bearer ') ? token : `Bearer ${token}`
}

export function createAuthMiddleware(options: ApiClientOptions): Middleware {
  const getToken = options.getToken ?? (() => tokenStore.get())

  return {
    pre: async (context) => {
      const token = await getToken()
      if (!token) return context

      const headers = new Headers(context.init.headers as HeadersInit)
      headers.set('Authorization', formatAuthorization(token))

      return {
        url: context.url,
        init: {
          ...context.init,
          headers,
        },
      }
    },
    post: async (context) => {
      const { response } = context
      if (response.status === 401) {
        options.onUnauthorized?.(response)
      }
      if (response.status === 403) {
        options.onForbidden?.(response)
      }
      return response
    },
  }
}

export function createApiConfiguration(options: ApiClientOptions = {}): Configuration {
  const params: ConfigurationParameters = {
    basePath: options.basePath,
    credentials: options.credentials ?? 'include',
    headers: options.headers,
    middleware: [createAuthMiddleware(options), ...(options.extraMiddleware ?? [])],
  }

  return new Configuration(params)
}
