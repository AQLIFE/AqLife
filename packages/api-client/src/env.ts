export function resolveApiBasePath(
  envValue?: string,
  fallback = 'https://localhost:7110',
): string {
  const base = envValue?.trim() || fallback
  return base.replace(/\/+$/, '')
}
