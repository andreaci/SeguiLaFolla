import { apiUrl } from '../config/api'
import type { AuthResponse, GameState, GameSummary, UserInfo } from '../types'

const AUTH_HEADER = 'X-Auth-Token'

function getToken(): string | null {
  return localStorage.getItem('mandria_token')
}

export function setToken(token: string | null) {
  if (token) localStorage.setItem('mandria_token', token)
  else localStorage.removeItem('mandria_token')
}

async function request<T>(
  path: string,
  options: RequestInit = {},
  auth = true
): Promise<T> {
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(options.headers as Record<string, string>),
  }
  if (auth) {
    const token = getToken()
    if (token) headers[AUTH_HEADER] = token
  }

  const res = await fetch(apiUrl(path), {
    ...options,
    headers,
    credentials: 'omit',
  })
  if (!res.ok) {
    const body = await res.json().catch(() => ({}))
    throw new Error((body as { error?: string }).error ?? res.statusText)
  }
  if (res.status === 204) return undefined as T
  return res.json() as Promise<T>
}

export const api = {
  register: (username: string, password: string, displayName: string) =>
    request<AuthResponse>('/api/auth/register', {
      method: 'POST',
      body: JSON.stringify({ username, password, displayName }),
    }, false),

  login: (username: string, password: string) =>
    request<AuthResponse>('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ username, password }),
    }, false),

  guest: (displayName: string) =>
    request<AuthResponse>('/api/auth/guest', {
      method: 'POST',
      body: JSON.stringify({ displayName }),
    }, false),

  me: () => request<UserInfo>('/api/auth/me'),

  listGames: () => request<GameSummary[]>('/api/games'),

  createGame: (name: string) =>
    request<GameState>('/api/games', {
      method: 'POST',
      body: JSON.stringify({ name }),
    }),

  getGame: (id: string) => request<GameState>(`/api/games/${id}`),

  joinGame: (id: string) =>
    request<GameState>(`/api/games/${id}/join`, { method: 'POST' }),

  leaveGame: (id: string) =>
    request<void>(`/api/games/${id}/leave`, { method: 'POST' }),

  startRound: (id: string) =>
    request<GameState>(`/api/games/${id}/round/start`, { method: 'POST' }),

  submitAnswer: (id: string, text: string) =>
    request<GameState>(`/api/games/${id}/answer`, {
      method: 'POST',
      body: JSON.stringify({ text }),
    }),

  submitVote: (id: string, answerId: string) =>
    request<GameState>(`/api/games/${id}/vote`, {
      method: 'POST',
      body: JSON.stringify({ answerId }),
    }),

  endVoting: (id: string) =>
    request<GameState>(`/api/games/${id}/round/end-voting`, { method: 'POST' }),

  forceEndTurn: (id: string) =>
    request<GameState>(`/api/games/${id}/round/force-end`, { method: 'POST' }),

  toLobby: (id: string) =>
    request<GameState>(`/api/games/${id}/round/to-lobby`, { method: 'POST' }),
}
