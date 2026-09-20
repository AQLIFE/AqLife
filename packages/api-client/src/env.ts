export function resolveApiBasePath(
  envValue?: string,
  fallback = 'http://localhost:5110',
): string {
  const base = envValue?.trim() || fallback
  console.log("ENV GET=>",envValue)
  return base.replace(/\/+$/, '')
}
