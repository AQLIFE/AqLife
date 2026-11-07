export function resolveApiBasePath(
  envValue?: string,
  fallback?: string,
): string {
  const browserFallback =
    typeof window === 'undefined'
      ? 'http://localhost:5110'
      : `${window.location.protocol}//${window.location.hostname}:${window.location.protocol === 'https:' ? '7110' : '5110'}`

  const base = envValue?.trim() || fallback || browserFallback
  return base.replace(/\/+$/, '')
}
