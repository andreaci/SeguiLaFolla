export function hasActivePenalty(
  userId: string,
  activePenaltyUserId?: string | null
): boolean {
  if (!activePenaltyUserId) return false
  return userId === activePenaltyUserId
}
