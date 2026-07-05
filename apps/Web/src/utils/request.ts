import { handleApiDetailed, ApiClientError } from '@aqlife/api-client'
import { ElMessage } from 'element-plus'

export { ApiClientError }

export async function handle<T>(promise: Promise<T>): Promise<[T | null, boolean]> {
  const [data, ok, error] = await handleApiDetailed(promise)
  if (!ok && error) {
    ElMessage.error(error.message)
  }
  return [data, ok]
}
