export interface ApiErrorBody {
  message?: string
  status?: number
  title?: string
  detail?: string
}

export class ApiClientError extends Error {
  readonly status: number
  readonly body?: ApiErrorBody | unknown

  constructor(message: string, status = 0, body?: unknown) {
    super(message)
    this.name = 'ApiClientError'
    this.status = status
    this.body = body
  }
}

export async function toApiClientError(error: unknown): Promise<ApiClientError> {
  if (error instanceof ApiClientError) {
    return error
  }

  if (isResponseError(error)) {
    const response = error.response
    let body: unknown
    let message = error.message || '请求失败'

    try {
      const contentType = response.headers.get('content-type') ?? ''
      if (contentType.includes('application/json')) {
        body = await response.clone().json()
        message = extractMessage(body) ?? message
      } else {
        const text = await response.clone().text()
        if (text) message = text
      }
    } catch {
      // ignore parse errors, keep default message
    }

    return new ApiClientError(message, response.status, body)
  }

  if (error instanceof Error) {
    return new ApiClientError(error.message)
  }

  return new ApiClientError('未知错误')
}

function extractMessage(body: unknown): string | undefined {
  if (!body || typeof body !== 'object') return undefined
  const record = body as Record<string, unknown>
  if (typeof record.message === 'string' && record.message) return record.message
  if (typeof record.title === 'string' && record.title) return record.title
  if (typeof record.detail === 'string' && record.detail) return record.detail
  return undefined
}

function isResponseError(error: unknown): error is { response: Response; message?: string } {
  return (
    typeof error === 'object'
    && error !== null
    && 'response' in error
    && (error as { response: unknown }).response instanceof Response
  )
}
