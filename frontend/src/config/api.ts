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
  return base ? `${base}${normalized}` : normalized
}

export function hubUrl(): string {
  return apiUrl('/hubs/game')
}
