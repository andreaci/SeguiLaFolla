import type { PlayerState } from '../types'

export function normalizePlayerName(name: string): string {
  return name.trim()
}

export function isDisplayNameTakenInGame(
  players: PlayerState[],
  name: string,
  excludeUserId?: string
): boolean {
  const normalized = normalizePlayerName(name)
  if (!normalized) return false
  return players.some(
    (p) =>
      p.userId !== excludeUserId &&
      normalizePlayerName(p.displayName).localeCompare(normalized, undefined, {
        sensitivity: 'accent',
      }) === 0
  )
}
