import { ApiClientError, toApiClientError } from './errors'

export type ApiResult<T> = [T, true] | [null, false, ApiClientError]

/** 兼容现有 [data, ok] 二元组用法 */
export async function handleApi<T>(promise: Promise<T>): Promise<[T | null, boolean]> {
  const [data, ok] = await handleApiDetailed(promise)
  return [data, ok]
}

export async function handleApiDetailed<T>(promise: Promise<T>): Promise<ApiResult<T>> {
  try {
    const data = await promise
    return [data, true]
  } catch (error) {
    const apiError = await toApiClientError(error)
    return [null, false, apiError]
  }
}
