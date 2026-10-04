/**
 * API base URL from environment (VITE_API_BASE_URL).
 * Empty = same-origin: in dev, Vite reverse-proxies /api and /hubs (no CORS).
 */
export function getApiBaseUrl(): string {
  const raw = import.meta.env.VITE_API_BASE_URL ?? ''
  return raw.replace(/\/$/, '')
}

export function apiUrl(path: string): string {
  const base = getApiBaseUrl()
  const normalized = path.startsWith('/') ? path : `/${path}`
  if (base) return `${base}${normalized}`

  const appBase = import.meta.env.BASE_URL.replace(/\/$/, '')
  return `${appBase}${normalized}`
}

export function hubUrl(): string {
  return apiUrl('/hubs/game')
}
