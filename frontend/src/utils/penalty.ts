function normalizeId(id: string): string {
  return id.replace(/-/g, '').toLowerCase()
}

export function hasActivePenalty(
  userId: string,
  activePenaltyUserId?: string | null
): boolean {
  if (!activePenaltyUserId || !userId) return false
  return normalizeId(userId) === normalizeId(activePenaltyUserId)
}

export function playerHasPoop(
  player: { userId: string; hasActivePenalty?: boolean },
  activePenaltyUserId?: string | null
): boolean {
  if (player.hasActivePenalty) return true
  return hasActivePenalty(player.userId, activePenaltyUserId)
}
