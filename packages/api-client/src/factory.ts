import type { BaseAPI, Configuration } from '@aqlife/api-contract'
import { createApiConfiguration, type ApiClientOptions } from './configuration'

type ApiConstructor<T extends BaseAPI> = new (configuration?: Configuration) => T

export function createApi<T extends BaseAPI>(
  ApiClass: ApiConstructor<T>,
  options?: ApiClientOptions,
): T {
  return new ApiClass(createApiConfiguration(options))
}
